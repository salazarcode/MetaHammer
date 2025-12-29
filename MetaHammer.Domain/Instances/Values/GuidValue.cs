using MetaHammer.Domain.Instances.Interfaces;

namespace MetaHammer.Domain.Instances.Values;

public class GuidValue(Guid value) : IPrimitiveValue
{
    public Guid Value { get; } = value;
}
