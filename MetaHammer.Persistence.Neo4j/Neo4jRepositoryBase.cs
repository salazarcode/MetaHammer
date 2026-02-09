using MetaHammer.Domain.Common;
using Neo4j.Driver;

namespace MetaHammer.Persistence.Neo4j;

public abstract class Neo4jRepositoryBase<T> where T : AggregateRoot
{
    protected readonly Neo4jContext Context;

    protected Neo4jRepositoryBase(Neo4jContext context)
    {
        Context = context;
    }

    public async Task SaveAsync(T aggregate)
    {
        await using var session = Context.CreateSession();
        await session.ExecuteWriteAsync(async tx =>
        {
            foreach (var @event in aggregate.DomainEvents)
            {
                await ApplyEventAsync(tx, @event);
            }
        });
        aggregate.ClearDomainEvents();
    }

    protected abstract Task ApplyEventAsync(IAsyncQueryRunner tx, object @event);
}
