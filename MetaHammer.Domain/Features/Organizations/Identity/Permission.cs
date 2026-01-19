using MetaHammer.Domain.Common;

namespace MetaHammer.Domain.Features.Organizations.Identity;

public class Permission(Guid guid, string name) : Entity(guid)
{
    //Un permiso tiene un nombre
    public string Name { get; private set; } = name;
}