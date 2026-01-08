using MetaHammer.Domain.Exceptions;
using MetaHammer.Domain.Types.Base;
using MetaHammer.Domain.Types.Entities;
using MetaHammer.Domain.Types.Interfaces;

namespace MetaHammer.Domain.Types;

public class ValueObjectType : MetaTypeWithProperties, IPropertyType
{
    public ValueObjectType(Guid guid, string name) : base(guid, name)
    {
    }
    
    public ValueObjectType(string name) : base(Guid.NewGuid(), name)
    {
    }
    

}