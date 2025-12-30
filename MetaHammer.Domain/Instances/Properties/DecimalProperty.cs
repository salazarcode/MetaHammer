namespace MetaHammer.Domain.Instances.Properties;

public class DecimalProperty(string propertyName, decimal value) : PropertyBase(propertyName)
{
    public decimal Value { get; } = value;
}
