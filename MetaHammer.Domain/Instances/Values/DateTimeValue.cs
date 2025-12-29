using MetaHammer.Domain.Instances.Interfaces;

namespace MetaHammer.Domain.Instances.Values;

public class DateTimeValue(DateTime value) : IPropertyValue
{
    public DateTime Value { get; } = value;
}
