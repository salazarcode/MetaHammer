using MetaHammer.Domain.Instances.Interfaces;

namespace MetaHammer.Domain.Instances.Values;

public class BoolValue(bool value) : IPrimitiveValue
{
    public bool Value { get; } = value;
}
