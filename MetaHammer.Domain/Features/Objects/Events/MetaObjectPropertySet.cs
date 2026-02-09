using MetaHammer.Domain.Interfaces;

namespace MetaHammer.Domain.Features.Objects.Events;

public record MetaObjectPropertySet(Guid ObjectGuid, string PropertyName, object? Value, Guid? ReferenceGuid) : IDomainEvent;
