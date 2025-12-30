using MetaHammer.Domain.Exceptions;
using MetaHammer.Domain.Instances.Interfaces;
using MetaHammer.Domain.Instances.Properties;
using MetaHammer.Domain.Types;
using MetaHammer.Domain.Types.Abstract;
using MetaHammer.Domain.Types.Interfaces;

namespace MetaHammer.Domain.Instances.Abstract;

public abstract class MetaInstanceWithProperties : MetaInstance
{
    private List<IProperty> Properties { get; } = new();
    public IReadOnlyCollection<IProperty> PropertiesAsReadOnly => Properties.AsReadOnly();

    public void SetPropertyValue(string propertyName, int value)
    {
        var definition = ValidatePrimitiveType(propertyName, PrimitiveType.Int);
        ValidateCanAddProperty(propertyName, definition);
        Properties.Add(new IntProperty(propertyName, value));
    }

    public void SetPropertyValue(string propertyName, string value)
    {
        var definition = ValidatePrimitiveType(propertyName, PrimitiveType.String);
        ValidateCanAddProperty(propertyName, definition);
        Properties.Add(new StringProperty(propertyName, value));
    }

    public void SetPropertyValue(string propertyName, bool value)
    {
        var definition = ValidatePrimitiveType(propertyName, PrimitiveType.Bool);
        ValidateCanAddProperty(propertyName, definition);
        Properties.Add(new BoolProperty(propertyName, value));
    }

    public void SetPropertyValue(string propertyName, decimal value)
    {
        var definition = ValidatePrimitiveType(propertyName, PrimitiveType.Decimal);
        ValidateCanAddProperty(propertyName, definition);
        Properties.Add(new DecimalProperty(propertyName, value));
    }

    public void SetPropertyValue(string propertyName, double value)
    {
        var definition = ValidatePrimitiveType(propertyName, PrimitiveType.Double);
        ValidateCanAddProperty(propertyName, definition);
        Properties.Add(new DoubleProperty(propertyName, value));
    }

    public void SetPropertyValue(string propertyName, DateTime value)
    {
        var definition = ValidatePrimitiveType(propertyName, PrimitiveType.DateTime);
        ValidateCanAddProperty(propertyName, definition);
        Properties.Add(new DateTimeProperty(propertyName, value));
    }

    public void SetPropertyValue(string propertyName, Guid value)
    {
        var definition = ValidatePrimitiveType(propertyName, PrimitiveType.Guid);
        ValidateCanAddProperty(propertyName, definition);
        Properties.Add(new GuidProperty(propertyName, value));
    }

    public void SetPropertyValue(string propertyName, ValueObjectInstance value)
    {
        var definition = FindPropertyDefinition(propertyName);

        if (definition.Type is not ValueObjectType expectedType)
            throw new DomainException($"Property '{propertyName}' is not a ValueObject type, it expects '{definition.Type.GetType().Name}'");

        if (value.Type != expectedType)
            throw new DomainException($"Property '{propertyName}' expects ValueObject of type '{expectedType.Name}', but received '{value.Type.Name}'");

        ValidateCanAddProperty(propertyName, definition);
        Properties.Add(new ValueObjectProperty(propertyName, value));
    }

    private PropertyDefinition ValidatePrimitiveType(string propertyName, PrimitiveType expectedType)
    {
        var definition = FindPropertyDefinition(propertyName);

        if (definition.Type is not PrimitiveType actualType)
            throw new DomainException($"Property '{propertyName}' is not a primitive type, it expects '{definition.Type.GetType().Name}'");

        if (actualType != expectedType)
            throw new DomainException($"Property '{propertyName}' expects primitive type '{actualType.Name}', but received '{expectedType.Name}'");

        return definition;
    }

    private void ValidateCanAddProperty(string propertyName, PropertyDefinition definition)
    {
        if (!definition.IsArray && Properties.Any(p => p.Name == propertyName))
            throw new DomainException($"Property '{propertyName}' already has a value. Use IsArray=true to allow multiple values.");
    }

    private PropertyDefinition FindPropertyDefinition(string propertyName)
    {
        var structuralType = (IStructuralType)Type;
        var definition = structuralType.Properties.FirstOrDefault(p => p.Name == propertyName);

        if (definition == null)
            throw new DomainException($"Property '{propertyName}' does not exist in type '{Type.Name}'");

        return definition;
    }
}
