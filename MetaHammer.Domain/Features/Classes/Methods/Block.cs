using MetaHammer.Domain.Common;

namespace MetaHammer.Domain.Features.Classes.Methods;

public class Block : Entity
{
    public List<BaseStatement> Statements { get; set; }
    public Scope Scope { get; set; } 
    public Block(Guid guid) : base(guid)
    {
        Scope = new Scope(Guid.NewGuid());
    }
}