using MetaHammer.Domain.Common;

namespace MetaHammer.Domain.Features.Organizations.Identity;

public class Person : Entity
{
    //Primer nombre
    public string FirstName { get; set; }
    
    //Apellido
    public string LastName { get; set; }
    
    //Constructor de persona con Guid
    public Person(Guid guid) : base(guid)
    {
    }
}