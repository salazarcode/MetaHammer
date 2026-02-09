using MetaHammer.Domain.Features.Classes.Enums;
using MetaHammer.Domain.Interfaces;

namespace MetaHammer.Domain.Features.Classes.Events;

public record MetaClassCreated(Guid Guid, string Name, MetaNature Nature, Guid OrganizationId, bool IsNative) : IDomainEvent;
