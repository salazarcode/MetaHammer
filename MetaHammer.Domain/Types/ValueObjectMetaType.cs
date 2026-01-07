using MetaHammer.Domain.Exceptions;
using MetaHammer.Domain.Types.Base;
using MetaHammer.Domain.Types.Entities;
using MetaHammer.Domain.Types.Interfaces;

namespace MetaHammer.Domain.Types;

public class ValueObjectMetaType : MetaTypeWithProperties, IPropertyType
{
    public ValueObjectMetaType(Guid guid, string name) : base(guid, name)
    {
    }
    
    public ValueObjectMetaType(string name) : base(Guid.NewGuid(), name)
    {
    }
    

}