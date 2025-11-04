using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Repository.Neo4j.Interfaces;
using Microsoft.Extensions.Logging;
using Neo4j.Driver;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Infrastructure.Repository.Neo4j.Repositories.Base;

/// <summary>
/// Implementación base abstracta para repositorios Neo4j.
/// Proporciona funcionalidad CRUD común para nodos y métodos protegidos para gestionar relaciones.
/// </summary>
public abstract class RepositoryBase<TEntity, TId> : IRepository<TEntity, TId> where TEntity : class
{
    protected readonly INeo4JDataAccess DataAccess;
    protected readonly ILogger Logger;

    /// <summary>
    /// La etiqueta principal del nodo en Neo4j (ej. "User", "Type").
    /// </summary>
    protected abstract string NodeLabel { get; }

    /// <summary>
    /// El nombre de la propiedad que actúa como identificador único (ej. "Guid").
    /// </summary>
    protected virtual string IdPropertyName => "Guid";

    protected RepositoryBase(INeo4JDataAccess dataAccess, ILogger logger)
    {
        DataAccess = dataAccess ?? throw new ArgumentNullException(nameof(dataAccess));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // ---
    // --- Métodos Abstractos (A ser implementados por repositorios especializados)
    // ---

    /// <summary>
    /// Mapea un INode de Neo4j a la entidad del dominio.
    /// </summary>
    protected abstract TEntity MapFromNode(INode node);

    /// <summary>
    /// Mapea una entidad del dominio a un diccionario de parámetros para Cypher.
    /// </summary>
    protected abstract Dictionary<string, object?> MapToParameters(TEntity entity);

    /// <summary>
    /// Obtiene el valor del identificador de una entidad.
    /// </summary>
    protected abstract TId GetEntityId(TEntity entity);

    // ---
    // --- Métodos CRUD de Nodos (Implementación genérica)
    // ---

    public virtual async Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var parameters = MapToParameters(entity);
        
        // El parámetro Cypher se llama '$properties' para evitar colisión con 'params'
        var queryString = $@"
            CREATE (n:{NodeLabel})
            SET n = $properties
            RETURN n";

        // Creamos un objeto Query que encapsula la consulta y los parámetros.
        var query = new Query(queryString, new { properties = parameters });

        return await DataAccess.ExecuteWriteAsync(async tx =>
        {
            var cursor = await tx.RunAsync(query);
            // SingleAsync() no usa CT; la cancelación es manejada por ExecuteWriteAsync
            var record = await cursor.SingleAsync(); 
            Logger.LogInformation("{NodeLabel} with ID {Id} created.", NodeLabel, GetEntityId(entity));
            return MapFromNode(record["n"].As<INode>());
        }, cancellationToken);
    }

    public virtual async Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        var query = $@"
            MATCH (n:{NodeLabel})
            WHERE n.`{IdPropertyName}` = $id
            RETURN n";

        return await DataAccess.ExecuteReadAsync(async tx =>
        {
            object idParam = id is Guid g ? g.ToString() : id!;
            var cursor = await tx.RunAsync(query, new { id = idParam });
            
            // Usamos ToListAsync().FirstOrDefault() para emular SingleOrDefault
            var records = await cursor.ToListAsync(cancellationToken);
            var record = records.FirstOrDefault();
            
            return record != null ? MapFromNode(record["n"].As<INode>()) : null;
        }, cancellationToken);
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var query = $"MATCH (n:{NodeLabel}) RETURN n";
        return await DataAccess.ExecuteReadAsync(async tx =>
        {
            var cursor = await tx.RunAsync(query);
            var records = await cursor.ToListAsync(cancellationToken);
            return records.Select(record => MapFromNode(record["n"].As<INode>()));
        }, cancellationToken);
    }

    public virtual async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var parameters = MapToParameters(entity);
        var id = GetEntityId(entity);
        object idParam = id is Guid g ? g.ToString() : id!;
        
        // El parámetro se llama 'props' para evitar colisiones
        var setClauses = string.Join(", ", parameters.Keys.Where(k => k != IdPropertyName).Select(k => $"n.`{k}` = $props.`{k}`"));
        
        var queryString = $@"
             MATCH (n:{NodeLabel} {{`{IdPropertyName}`: $id}})
             SET {setClauses}
             RETURN n";

        var query = new Query(queryString, new { id = idParam, props = parameters });

        return await DataAccess.ExecuteWriteAsync(async tx =>
        {
            var cursor = await tx.RunAsync(query);
            var record = await cursor.SingleAsync(); // No usa CT
            Logger.LogInformation("{NodeLabel} with ID {Id} updated.", NodeLabel, id);
            return MapFromNode(record["n"].As<INode>());
        }, cancellationToken);
    }

    public virtual async Task<bool> DeleteAsync(TId id, CancellationToken cancellationToken = default)
    {
        var query = $@"
             MATCH (n:{NodeLabel} {{`{IdPropertyName}`: $id}})
             DETACH DELETE n
             RETURN count(n) as deletedCount"; // Devolvemos un conteo

        object idParam = id is Guid g ? g.ToString() : id!;

        return await DataAccess.ExecuteWriteAsync(async tx =>
        {
            var cursor = await tx.RunAsync(query, new { id = idParam });
            var record = await cursor.SingleAsync(); // No usa CT
            var deletedCount = record["deletedCount"].As<long>();

            if (deletedCount > 0)
            {
                Logger.LogInformation("{NodeLabel} with ID {Id} deleted.", NodeLabel, id);
                return true;
            }
            
            Logger.LogWarning("{NodeLabel} with ID {Id} not found for deletion.", NodeLabel, id);
            return false;
        }, cancellationToken);
    }

    // ---
    // --- Métodos de Relación Protegidos (Para uso de repositorios especializados)
    // ---

    /// <summary>
    /// Crea una nueva relación entre dos nodos cualesquiera, identificados por sus Guids.
    /// </summary>
    protected async Task CreateRelationshipAsync(
        Guid fromNodeId, 
        Guid toNodeId, 
        string relationshipType, 
        Dictionary<string, object?> properties, 
        CancellationToken cancellationToken = default)
    {
        var sanitizedRelType = $"`{relationshipType.ToUpper()}`";
        var query = $@"
            MATCH (from) WHERE from.`{IdPropertyName}` = $fromId
            MATCH (to) WHERE to.`{IdPropertyName}` = $toId
            CREATE (from)-[r:{sanitizedRelType}]->(to)
            SET r = $properties";

        await DataAccess.ExecuteWriteAsync(async tx =>
        {
            await tx.RunAsync(query, new
            {
                fromId = fromNodeId.ToString(),
                toId = toNodeId.ToString(),
                properties
            });
        }, cancellationToken);
    }

    /// <summary>
    /// Elimina una relación entre dos nodos basada en sus Guids y el tipo de relación.
    /// </summary>
    protected async Task DeleteRelationshipAsync(
        Guid fromNodeId, 
        Guid toNodeId, 
        string relationshipType, 
        CancellationToken cancellationToken = default)
    {
        var sanitizedRelType = $"`{relationshipType.ToUpper()}`";
        var query = $@"
            MATCH (from {{`{IdPropertyName}`: $fromId}})-[r:{sanitizedRelType}]->(to {{`{IdPropertyName}`: $toId}})
            DELETE r";

        await DataAccess.ExecuteWriteAsync(async tx =>
        {
            await tx.RunAsync(query, new
            {
                fromId = fromNodeId.ToString(),
                toId = toNodeId.ToString()
            });
        }, cancellationToken);
    }
}
