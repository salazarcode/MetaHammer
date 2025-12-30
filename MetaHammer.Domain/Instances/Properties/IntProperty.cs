namespace MetaHammer.Domain.Instances.Properties;

public class IntProperty(string propertyName, int value) : PropertyBase(propertyName)
{
    public int Value { get; } = value;
}
