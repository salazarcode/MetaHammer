using MetaHammer.Domain.Common;
using MetaHammer.Domain.Types.Base;
using MetaHammer.Domain.Types.Entities.Method;
using MetaHammer.Domain.Types.Interfaces;
using MetaHammer.Domain.Validation;

namespace MetaHammer.Domain.Types;

public class PrimitiveMetaType : MetaType, IPropertyType
{
    public PrimitiveMetaType(Guid guid, string name) : base(guid, name)
    {
    }

    public PrimitiveMetaType(string name) : base(Guid.NewGuid(), name)
    {
    }
}