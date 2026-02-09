using MetaHammer.Domain.Interfaces;

namespace MetaHammer.Domain.Features.Classes.Events;

public record MetaPropertyAdded(Guid ClassGuid, string PropertyName, Guid PropertyTypeGuid, bool IsCollection) : IDomainEvent;
