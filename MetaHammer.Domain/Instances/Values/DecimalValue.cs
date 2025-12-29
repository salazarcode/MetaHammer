using MetaHammer.Domain.Instances.Interfaces;

namespace MetaHammer.Domain.Instances.Values;

public class DecimalValue(decimal value) : IPrimitiveValue
{
    public decimal Value { get; } = value;
}
