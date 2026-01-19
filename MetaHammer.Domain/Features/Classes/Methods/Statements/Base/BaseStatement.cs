using MetaHammer.Domain.Common;
using MetaHammer.Domain.Exceptions;

namespace MetaHammer.Domain.Features.Classes.Methods;

public abstract class BaseStatement : Entity
{
    public BaseStatement(Guid guid) : base(guid)
    {
    }
}