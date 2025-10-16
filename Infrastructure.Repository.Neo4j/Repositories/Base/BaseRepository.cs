using Infrastructure.Repository.Neo4j.Interfaces;
using Microsoft.Extensions.Logging;
using Neo4j.Driver;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Infrastructure.Repository.Neo4j.Repositories.Base;

public abstract class RepositoryBase<TEntity, TId> : IRepository<TEntity, TId> where TEntity : class
{
    protected readonly INeo4jDataAccess DataAccess;
    protected readonly ILogger Logger;

    protected abstract string NodeLabel { get; }
    protected virtual string IdPropertyName => "Guid";

    protected RepositoryBase(INeo4jDataAccess dataAccess, ILogger logger)
    {
        DataAccess = dataAccess ?? throw new ArgumentNullException(nameof(dataAccess));
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected abstract TEntity MapFromNode(INode node);
    protected abstract Dictionary<string, object?> MapToParameters(TEntity entity);
    protected abstract TId GetEntityId(TEntity entity);

    public virtual async Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var parameters = MapToParameters(entity);

        var queryString = $@"
        CREATE (n:{NodeLabel})
        SET n = $properties
        RETURN n";

        return await DataAccess.ExecuteWriteAsync(async tx =>
        {
            var cursor = await tx.RunAsync(queryString, new{propierties = parameters});
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
            var cursor = await tx.RunAsync(query, new { id });
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
        var setClauses = string.Join(", ", parameters.Keys.Where(k => k != IdPropertyName).Select(k => $"n.`{k}` = $props.`{k}`"));
        
        var query = $@"
            MATCH (n:{NodeLabel} {{`{IdPropertyName}`: $id}})
            SET {setClauses}
            RETURN n";

        return await DataAccess.ExecuteWriteAsync(async tx =>
        {
            var cursor = await tx.RunAsync(query, new { id, props = parameters });
            var record = await cursor.SingleAsync();
            return MapFromNode(record["n"].As<INode>());
        }, cancellationToken);
    }

    public virtual async Task<bool> DeleteAsync(TId id, CancellationToken cancellationToken = default)
    {
        var query = $@"
            MATCH (n:{NodeLabel} {{`{IdPropertyName}`: $id}})
            DETACH DELETE n";

        await DataAccess.ExecuteWriteAsync(tx => tx.RunAsync(query, new { id }), cancellationToken);
        Logger.LogInformation("{NodeLabel} with ID {Id} deleted.", NodeLabel, id);
        return true; // Asumimos éxito si no hay excepción. Ver nota de refinamiento.
    }
}
