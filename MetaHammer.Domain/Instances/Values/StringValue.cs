using MetaHammer.Domain.Instances.Interfaces;

namespace MetaHammer.Domain.Instances.Values;

public class StringValue(string value) : IPrimitiveValue
{
    public string Value { get; } = value;
}
