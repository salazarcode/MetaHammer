namespace MetaHammer.Domain.Instances.Properties;

public class DateTimeProperty(string propertyName, DateTime value) : PropertyBase(propertyName)
{
    public DateTime Value { get; } = value;
}
