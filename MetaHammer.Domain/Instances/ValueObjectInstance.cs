using MetaHammer.Domain.Instances.Abstract;
using MetaHammer.Domain.Instances.Interfaces;
using MetaHammer.Domain.Types;

namespace MetaHammer.Domain.Instances;

public class ValueObjectInstance : MetaInstanceWithProperties, IPropertyValue
{
    public ValueObjectInstance(ValueObjectType type)
    {
        Guid = Guid.NewGuid();
        Type = type;
    }
}
