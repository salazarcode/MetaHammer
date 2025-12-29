using MetaHammer.Domain.Instances.Interfaces;

namespace MetaHammer.Domain.Instances.Values;

public class IntValue(int value) : IPrimitiveValue
{
    public int Value { get; } = value;
}
