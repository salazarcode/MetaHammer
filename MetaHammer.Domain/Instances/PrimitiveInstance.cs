using MetaHammer.Domain.Instances.Abstract;
using MetaHammer.Domain.Instances.Interfaces;
using MetaHammer.Domain.Types;

namespace MetaHammer.Domain.Instances;

public class PrimitiveInstance : MetaInstance, IPropertyValue
{
    public required IPrimitiveValue Value { get; init; }

    public PrimitiveInstance(PrimitiveType type, IPrimitiveValue value)
    {
        Type = type;
        Value = value;
    }
}
