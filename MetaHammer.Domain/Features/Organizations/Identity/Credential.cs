using MetaHammer.Domain.Common;

namespace MetaHammer.Domain.Features.Organizations.Identity;

public class Credential: Entity
{
    //Una credencial tiene username
    public string Username { get; private set; }
    
    //Una credencial tiene passwordHash
    public string PasswordHash { get; private set; }
    
    //Una credencial tiene salt
    public string Salt { get; private set; }
    
    //Una credencial tiene un constructor
    public Credential(Guid guid, string username) : base(guid)
    {
        Username = username;
    }
}