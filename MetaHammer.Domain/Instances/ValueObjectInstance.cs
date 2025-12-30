using MetaHammer.Domain.Instances.Abstract;
using MetaHammer.Domain.Instances.Interfaces;
using MetaHammer.Domain.Types;

namespace MetaHammer.Domain.Instances;

public class ValueObjectInstance : MetaInstanceWithProperties, IInstanceProperty
{
    public ValueObjectInstance(ValueObjectType type)
    {
        Type = type;
    }
}
