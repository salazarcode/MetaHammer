using MetaHammer.Domain.Instances;
using MetaHammer.Domain.Types.Abstract;
using MetaHammer.Domain.Types.Interfaces;
using MetaHammer.Domain.Types.Methods;

namespace MetaHammer.Domain.Types;

public class ValueObjectType : StructuralType, IPropertyType
{
    public ValueObjectType(string name)
    {
        Guid = Guid.NewGuid();
        Name = name;
    }

    public ValueObjectInstance CreateInstance()
    {
        return new ValueObjectInstance(this);
    }
}