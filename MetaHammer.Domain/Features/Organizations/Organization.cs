using MetaHammer.Domain.Common;
using MetaHammer.Domain.Exceptions;
using MetaHammer.Domain.Features.Classes;
using MetaHammer.Domain.Features.Classes.Enums;
using MetaHammer.Domain.Features.Organizations.Identity;

namespace MetaHammer.Domain.Features.Organizations;

public class Organization : Entity
{
    public Organization(Guid guid, string name) : base(guid)
    {
        Name = name;
    }
    
    //Name
    public string Name { get; set; }
    
    //Tiene muchos tipos
    public List<MetaClass> Classes { get; set; } = new();
    
    //Tiene muchos usuarios
    public List<User> Users { get; set; } = new();
    
    //Tiene muchos grupos
    public List<Group> Groups { get; set; } = new();
    
    //Tiene muchos permisos
    public List<Permission> Permissions { get; set; } = new();
    
    //El organization tiene un metodo para agregar grupos
    public Group AddGroup(string name)
    {
        if (Groups.FirstOrDefault(x => x.Name == name) != null)
            throw new DomainException($"El organization ya tiene un permiso de nombre '{name}'");
        
        var group = new Group(Guid.NewGuid(), name);
        Groups.Add(group);
        return group;

    }
    
    //Metodo para agregar permisos al organization
    public Permission AddPermission(string name)
    {
        if (Permissions.FirstOrDefault(x => x.Name == name) != null)
            throw new DomainException($"El organization ya tiene un permiso de nombre '{name}'");

        var permission = new Permission(Guid.NewGuid(), name);
        Permissions.Add(permission);
        return permission;
    }

    public User AddUser(Guid guid, string userName, Guid? creatorGuid = null)
    {
        if (Users.FirstOrDefault(x => x.Credential.Username == userName) != null)
            throw new DomainException($"El organization ya tiene un permiso de userName '{userName}'");
        
        var user = new User(this, guid, userName);
        return user;
    }

    public MetaClass AddMetaClass(string name, MetaNature metaNature, User user)
    {
        if (Classes.Any(x => x.Name == name))
            throw new DomainException($"El organization ya tiene una clase de nombre '{name}'");
        
        var metaClass = new MetaClass(Guid.NewGuid(), name, metaNature, this, user);
        return metaClass;
    }
    
}