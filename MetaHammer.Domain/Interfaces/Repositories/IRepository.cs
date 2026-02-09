using MetaHammer.Domain.Common;

namespace MetaHammer.Domain.Interfaces.Repositories;

public interface IRepository<T> where T : AggregateRoot
{
    Task SaveAsync(T aggregate);
}
