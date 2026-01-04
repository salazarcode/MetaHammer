using MetaHammer.Domain.Types.Abstract;

namespace MetaHammer.Domain.Common;

public abstract class AggregateRoot : Entity
{
    public AggregateRoot(Guid guid) : base(guid)
    {
    }
    
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    public void ClearDomainEvents() => _domainEvents.Clear();
}