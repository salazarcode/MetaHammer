namespace MetaHammer.Domain.Instances.Properties;

public class GuidProperty(string propertyName, Guid value) : PropertyBase(propertyName)
{
    public Guid Value { get; } = value;
}
