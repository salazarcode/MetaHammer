namespace MetaHammer.Domain.Instances.Properties;

public class ValueObjectProperty(string propertyName, ValueObjectInstance value) : PropertyBase(propertyName)
{
    public ValueObjectInstance Value { get; } = value;
}
