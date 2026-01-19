using MetaHammer.Domain.Common;
using MetaHammer.Domain.Exceptions;

namespace MetaHammer.Domain.Features.Organizations.Identity;

public class Group(Guid guid, string name) : Entity(guid)
{
    //Un grupo tiene Name
    public string Name { get; set; } = name;
    //Un grupo tiene muchos permisos
    private List<Permission> Permissions { get; set; } = new();
    
    //Metodo que devuelve permisos de un grupo como readonly list
    public IReadOnlyList<Permission> GetPermissions() => Permissions.AsReadOnly();
    
    //Agregar un permiso al grupo
    public void AddPermission(Permission permission)
    {
        if (Permissions.Any(p => p.Guid == permission.Guid))
        {
            throw new DomainException($"El permiso con GUID '{permission.Guid}' ya existe en el grupo.");
        }
        
        Permissions.Add(permission);
    }
}