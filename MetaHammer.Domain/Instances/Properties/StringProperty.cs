namespace MetaHammer.Domain.Instances.Properties;

public class StringProperty(string propertyName, string value) : PropertyBase(propertyName)
{
    public string Value { get; } = value;
}
