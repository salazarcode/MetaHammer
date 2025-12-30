namespace MetaHammer.Domain.Instances.Properties;

public class DoubleProperty(string propertyName, double value) : PropertyBase(propertyName)
{
    public double Value { get; } = value;
}
