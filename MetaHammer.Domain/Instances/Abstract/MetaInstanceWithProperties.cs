using MetaHammer.Domain.Exceptions;
using MetaHammer.Domain.Instances.Interfaces;
using MetaHammer.Domain.Instances.Properties;
using MetaHammer.Domain.Types;
using MetaHammer.Domain.Types.Abstract;
using MetaHammer.Domain.Types.Interfaces;

namespace MetaHammer.Domain.Instances.Abstract;

/// <summary>
/// Clase base abstracta para instancias que pueden tener propiedades.
/// Proporciona métodos tipados para asignar valores a las propiedades definidas en el tipo.
/// </summary>
public abstract class MetaInstanceWithProperties : MetaInstance
{
    /// <summary>
    /// Lista interna de propiedades de la instancia.
    /// </summary>
    private List<IProperty> Properties { get; } = new();

    /// <summary>
    /// Colección de solo lectura de las propiedades de la instancia.
    /// </summary>
    public IReadOnlyCollection<IProperty> PropertiesAsReadOnly => Properties.AsReadOnly();

    /// <summary>
    /// Asigna un valor entero a una propiedad.
    /// </summary>
    /// <param name="propertyName">Nombre de la propiedad.</param>
    /// <param name="value">Valor entero a asignar.</param>
    /// <exception cref="DomainException">Si la propiedad no existe o el tipo no coincide.</exception>
    public void SetPropertyValue(string propertyName, int value)
    {
        var definition = ValidatePrimitiveType(propertyName, PrimitiveType.Int);
        ValidateCanAddProperty(propertyName, definition);
        Properties.Add(new IntProperty(propertyName, value));
    }

    /// <summary>
    /// Asigna un valor de texto a una propiedad.
    /// </summary>
    /// <param name="propertyName">Nombre de la propiedad.</param>
    /// <param name="value">Valor de texto a asignar.</param>
    /// <exception cref="DomainException">Si la propiedad no existe o el tipo no coincide.</exception>
    public void SetPropertyValue(string propertyName, string value)
    {
        var definition = ValidatePrimitiveType(propertyName, PrimitiveType.String);
        ValidateCanAddProperty(propertyName, definition);
        Properties.Add(new StringProperty(propertyName, value));
    }

    /// <summary>
    /// Asigna un valor booleano a una propiedad.
    /// </summary>
    /// <param name="propertyName">Nombre de la propiedad.</param>
    /// <param name="value">Valor booleano a asignar.</param>
    /// <exception cref="DomainException">Si la propiedad no existe o el tipo no coincide.</exception>
    public void SetPropertyValue(string propertyName, bool value)
    {
        var definition = ValidatePrimitiveType(propertyName, PrimitiveType.Bool);
        ValidateCanAddProperty(propertyName, definition);
        Properties.Add(new BoolProperty(propertyName, value));
    }

    /// <summary>
    /// Asigna un valor decimal a una propiedad.
    /// </summary>
    /// <param name="propertyName">Nombre de la propiedad.</param>
    /// <param name="value">Valor decimal a asignar.</param>
    /// <exception cref="DomainException">Si la propiedad no existe o el tipo no coincide.</exception>
    public void SetPropertyValue(string propertyName, decimal value)
    {
        var definition = ValidatePrimitiveType(propertyName, PrimitiveType.Decimal);
        ValidateCanAddProperty(propertyName, definition);
        Properties.Add(new DecimalProperty(propertyName, value));
    }

    /// <summary>
    /// Asigna un valor de punto flotante doble a una propiedad.
    /// </summary>
    /// <param name="propertyName">Nombre de la propiedad.</param>
    /// <param name="value">Valor double a asignar.</param>
    /// <exception cref="DomainException">Si la propiedad no existe o el tipo no coincide.</exception>
    public void SetPropertyValue(string propertyName, double value)
    {
        var definition = ValidatePrimitiveType(propertyName, PrimitiveType.Double);
        ValidateCanAddProperty(propertyName, definition);
        Properties.Add(new DoubleProperty(propertyName, value));
    }

    /// <summary>
    /// Asigna un valor de fecha y hora a una propiedad.
    /// </summary>
    /// <param name="propertyName">Nombre de la propiedad.</param>
    /// <param name="value">Valor DateTime a asignar.</param>
    /// <exception cref="DomainException">Si la propiedad no existe o el tipo no coincide.</exception>
    public void SetPropertyValue(string propertyName, DateTime value)
    {
        var definition = ValidatePrimitiveType(propertyName, PrimitiveType.DateTime);
        ValidateCanAddProperty(propertyName, definition);
        Properties.Add(new DateTimeProperty(propertyName, value));
    }

    /// <summary>
    /// Asigna un valor GUID a una propiedad.
    /// </summary>
    /// <param name="propertyName">Nombre de la propiedad.</param>
    /// <param name="value">Valor Guid a asignar.</param>
    /// <exception cref="DomainException">Si la propiedad no existe o el tipo no coincide.</exception>
    public void SetPropertyValue(string propertyName, Guid value)
    {
        var definition = ValidatePrimitiveType(propertyName, PrimitiveType.Guid);
        ValidateCanAddProperty(propertyName, definition);
        Properties.Add(new GuidProperty(propertyName, value));
    }

    /// <summary>
    /// Asigna un objeto de valor a una propiedad.
    /// </summary>
    /// <param name="propertyName">Nombre de la propiedad.</param>
    /// <param name="value">Instancia de ValueObject a asignar.</param>
    /// <exception cref="DomainException">Si la propiedad no existe o el tipo no coincide.</exception>
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

    /// <summary>
    /// Valida que la propiedad sea del tipo primitivo esperado.
    /// </summary>
    /// <param name="propertyName">Nombre de la propiedad.</param>
    /// <param name="expectedType">Tipo primitivo esperado.</param>
    /// <returns>Definición de la propiedad validada.</returns>
    /// <exception cref="DomainException">Si el tipo no coincide.</exception>
    private PropertyDefinition ValidatePrimitiveType(string propertyName, PrimitiveType expectedType)
    {
        var definition = FindPropertyDefinition(propertyName);

        if (definition.Type is not PrimitiveType actualType)
            throw new DomainException($"Property '{propertyName}' is not a primitive type, it expects '{definition.Type.GetType().Name}'");

        if (actualType != expectedType)
            throw new DomainException($"Property '{propertyName}' expects primitive type '{actualType.Name}', but received '{expectedType.Name}'");

        return definition;
    }

    /// <summary>
    /// Valida que se pueda agregar un valor a la propiedad.
    /// Si la propiedad no es un array y ya tiene valor, lanza excepción.
    /// </summary>
    /// <param name="propertyName">Nombre de la propiedad.</param>
    /// <param name="definition">Definición de la propiedad.</param>
    /// <exception cref="DomainException">Si la propiedad ya tiene valor y no es array.</exception>
    private void ValidateCanAddProperty(string propertyName, PropertyDefinition definition)
    {
        if (!definition.IsArray && Properties.Any(p => p.Name == propertyName))
            throw new DomainException($"Property '{propertyName}' already has a value. Use IsArray=true to allow multiple values.");
    }

    /// <summary>
    /// Busca la definición de una propiedad en el tipo estructural.
    /// </summary>
    /// <param name="propertyName">Nombre de la propiedad.</param>
    /// <returns>Definición de la propiedad encontrada.</returns>
    /// <exception cref="DomainException">Si la propiedad no existe en el tipo.</exception>
    private PropertyDefinition FindPropertyDefinition(string propertyName)
    {
        var structuralType = (IStructuralType)Type;
        var definition = structuralType.Properties.FirstOrDefault(p => p.Name == propertyName);

        if (definition == null)
            throw new DomainException($"Property '{propertyName}' does not exist in type '{Type.Name}'");

        return definition;
    }
}
