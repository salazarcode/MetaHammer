namespace MetaHammer.Domain.Instances.Properties;

public class BoolProperty(string propertyName, bool value) : PropertyBase(propertyName)
{
    public bool Value { get; } = value;
}
