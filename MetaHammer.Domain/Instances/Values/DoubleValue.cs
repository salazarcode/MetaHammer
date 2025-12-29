using MetaHammer.Domain.Instances.Interfaces;

namespace MetaHammer.Domain.Instances.Values;

public class DoubleValue(double value) : IPrimitiveValue
{
    public double Value { get; } = value;
}
