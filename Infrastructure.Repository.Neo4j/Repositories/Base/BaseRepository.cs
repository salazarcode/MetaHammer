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
/// Esta versión soporta la creación de nodos con múltiples etiquetas para polimorfismo.
/// </summary>
public abstract class RepositoryBase<TEntity, TId> : IRepository<TEntity, TId> where TEntity : class
{
    protected readonly INeo4JDataAccess DataAccess;
    protected readonly ILogger Logger;

    /// <summary>
    /// La etiqueta principal del nodo en Neo4j (ej. "User", "Type").
    /// Esta etiqueta se usa para TODAS las consultas de coincidencia (MATCH).
    /// </summary>
    protected abstract IEnumerable<string> NodeLabels { get; }

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
    // --- NUEVO MÉTODO (VIRTUAL) ---
    // ---

    /// <summary>
    /// Obtiene la lista de etiquetas para un nodo en el momento de la creación.
    /// Los repositorios especializados pueden sobrescribir esto para añadir etiquetas
    /// polimórficas (ej. :Type:ComplexType).
    /// </summary>
    /// <param name="entity">La entidad que se va a crear.</param>
    /// <returns>Una colección de strings de etiquetas.</returns>
    protected virtual IEnumerable<string> GetNodeLabels(TEntity entity)
    {
        // Por defecto, solo usa la etiqueta base.
        return new[] { NodeLabels.First() };
    }

    // ---
    // --- Métodos CRUD de Nodos (Implementación genérica)
    // ---

    public virtual async Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var parameters = MapToParameters(entity);
        
        // --- LÓGICA MODIFICADA ---
        // 1. Obtener todas las etiquetas (base y especializadas)
        var labels = GetNodeLabels(entity);
        // 2. Crear el string de etiquetas sanitizado para Cypher (ej. :`Type`:`ComplexType`)
        var labelString = string.Join(":", labels.Select(SanitizeLabel));
        // --- FIN DE LA MODIFICACIÓN ---

        var queryString = $@"
            CREATE (n:{labelString}) 
            SET n = $properties
            RETURN n";

        var query = new Query(queryString, new { properties = parameters });

        return await DataAccess.ExecuteWriteAsync(async tx =>
        {
            var cursor = await tx.RunAsync(query);
            var record = await cursor.SingleAsync(); 
            Logger.LogInformation("Nodo con etiquetas [{Labels}] e ID {Id} creado.", labelString, GetEntityId(entity));
            return MapFromNode(record["n"].As<INode>());
        }, cancellationToken);
    }

    // Los métodos GetById, GetAll, Update y Delete no cambian.
    // Usan 'NodeLabel' para las operaciones MATCH, que es eficiente
    // y encontrará los nodos independientemente de sus otras etiquetas.

    public virtual async Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
    {
        var labelsForMatch = string.Join(":", NodeLabels.Select(l => l.Trim()).Where(l => !string.IsNullOrWhiteSpace(l)));
        var query = $@"
            MATCH (n:{labelsForMatch})
            WHERE n.`{IdPropertyName}` = $id
            RETURN n";

        return await DataAccess.ExecuteReadAsync(async tx =>
        {
            object idParam = id is Guid g ? g.ToString() : id!;
            var cursor = await tx.RunAsync(query, new { id = idParam });
            var records = await cursor.ToListAsync(cancellationToken);
            var record = records.FirstOrDefault();
            return record != null ? MapFromNode(record["n"].As<INode>()) : null;
        }, cancellationToken);
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var labelsForMatch = string.Join(":", NodeLabels.Select(l => l.Trim()).Where(l => !string.IsNullOrWhiteSpace(l)));
        var query = $"MATCH (n:{labelsForMatch}) RETURN n";
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
        
        var setClauses = string.Join(", ", parameters.Keys.Where(k => k != IdPropertyName).Select(k => $"n.`{k}` = $props.`{k}`"));
        
        var labelsForMatch = string.Join(":", NodeLabels.Select(l => l.Trim()).Where(l => !string.IsNullOrWhiteSpace(l)));
        var queryString = $@"
             MATCH (n:{labelsForMatch} {{`{IdPropertyName}`: $id}})
             SET {setClauses}
             RETURN n";

        var neoQuery = new Query(queryString, new { id = idParam, props = parameters });

        return await DataAccess.ExecuteWriteAsync(async tx =>
        {
            var cursor = await tx.RunAsync(neoQuery);
            var record = await cursor.SingleAsync();
            Logger.LogInformation("Nodo con etiquetas [{Labels}] y ID {Id} actualizado.", labelsForMatch, id);
            return MapFromNode(record["n"].As<INode>());
        }, cancellationToken);
    }

    public virtual async Task<bool> DeleteAsync(TId id, CancellationToken cancellationToken = default)
    {
        var labelsForMatch = string.Join(":", NodeLabels.Select(l => l.Trim()).Where(l => !string.IsNullOrWhiteSpace(l)));
        var query = $@"
             MATCH (n:{labelsForMatch} {{`{IdPropertyName}`: $id}})
             DETACH DELETE n
             RETURN count(n) as deletedCount";

        object idParam = id is Guid g ? g.ToString() : id!;

        return await DataAccess.ExecuteWriteAsync(async tx =>
        {
            var cursor = await tx.RunAsync(query, new { id = idParam });
            var record = await cursor.SingleAsync();
            var deletedCount = record["deletedCount"].As<long>();

            if (deletedCount > 0)
            {
                Logger.LogInformation("Nodo(s) con etiquetas [{Labels}] y ID {Id} eliminado(s).", labelsForMatch, id);
                return true;
            }
            
            Logger.LogWarning("Nodo(s) con etiquetas [{Labels}] y ID {Id} no encontrado(s) para eliminación.", labelsForMatch, id);
            return false;
        }, cancellationToken);
    }

    // ---
    // --- Métodos de Relación Protegidos (Sin cambios)
    // ---
    
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

    // ---
    // --- NUEVO MÉTODO HELPER (PRIVADO) ---
    // ---

    /// <summary>
    /// Sanitiza un string para ser usado como etiqueta o tipo de relación en Cypher
    /// para prevenir inyecciones de Cypher, envolviéndolo en backticks.
    /// </summary>
    private string SanitizeLabel(string label)
    {
        // Validar entrada básica
        if (string.IsNullOrWhiteSpace(label))
        {
            throw new ArgumentException($"Nombre de etiqueta inválido: '{label}'");
        }

        // Rechazar carácteres que rompan la sintaxis Cypher: backtick y ':' (separador de etiquetas)
        if (label.Contains('`') || label.Contains(':') || label.Any(char.IsControl))
        {
            throw new ArgumentException($"Nombre de etiqueta inválido: '{label}'");
        }

        // Normalizar (quitar espacios sobrantes)
        var trimmed = label.Trim();

        // Envolvemos en backticks para soportar mayúsculas, guiones, espacios, etc.
        return $"`{trimmed}`";
    }
}
