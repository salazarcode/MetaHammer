using MetaHammer.Domain.Common;

namespace MetaHammer.Domain.Features.Organizations.Identity;

public class User : Entity
{
    public Guid TenantGuid { get; private set;  }
    public Organization Organization { get; private init; }
    
    public Guid? CreatedByGuid { get; private set;  }
    public User? CreatedBy { get; init; }
    
    
    public Guid CredentialGuid { get; private set; }
    public Credential Credential { get; set; }
    
    
    public Guid PersonGuid { get; private set;  }
    public Person Person { get; private set; }
    

    public Guid PrimaryGroupGuid { get; init; }
    public Group PrimaryGroup { get; private set; }
    
    
    //Un user tiene credenciales
    
    //Estado del usuario
    public bool IsActive { get; private set; }
    
    //Fecha de creacion del usuario
    public DateTime CreatedAt { get; private set; }
    
    //Un usuario puede pertenecer a muchos grupos
    public List<Group> Groups { get; private set; } = new();
    
    //Un usuario tiene credenciales

    public User(Organization organization, Guid guid, string userName, User? creator = null, bool isActive = true) : base(guid)
    {
        Person = new Person(Guid.NewGuid());
        PersonGuid = Person.Guid;
        
        Credential = new Credential(Guid.NewGuid(), userName);
        CredentialGuid = Credential.Guid;
        
        PrimaryGroup = new Group(Guid.NewGuid(), userName);
        PrimaryGroupGuid = PrimaryGroup.Guid;
        Groups.Add(PrimaryGroup);
        
        TenantGuid = organization.Guid;
        Organization = organization;
        
        CreatedBy = creator;
        CreatedByGuid = CreatedBy?.Guid;
        
        IsActive = isActive;
        CreatedAt = DateTime.UtcNow;
    }
    
    //Metodo que devuelve los permisos a los que tiene acceso el usuario a traves de sus grupos
    public IReadOnlyList<Permission> GetPermissions()
    {
        var permissions = new List<Permission>();
        
        foreach (var group in Groups)
        {
            permissions.AddRange(group.GetPermissions());
        }
        
        return permissions.Distinct().ToList().AsReadOnly();
    }
        
}