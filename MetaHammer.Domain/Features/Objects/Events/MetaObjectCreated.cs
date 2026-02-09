using MetaHammer.Domain.Interfaces;

namespace MetaHammer.Domain.Features.Objects.Events;

public record MetaObjectCreated(Guid Guid, Guid ClassGuid, Guid TenantGuid) : IDomainEvent;
