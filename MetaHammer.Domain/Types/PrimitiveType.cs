using MetaHammer.Domain.Common;
using MetaHammer.Domain.Types.Base;
using MetaHammer.Domain.Types.Entities.Method;
using MetaHammer.Domain.Types.Interfaces;
using MetaHammer.Domain.Validation;

namespace MetaHammer.Domain.Types;

public class PrimitiveType : MetaType, IPropertyType
{
    public PrimitiveType(Guid guid, string name) : base(guid, name)
    {
    }

    public PrimitiveType(string name) : base(Guid.NewGuid(), name)
    {
    }
}