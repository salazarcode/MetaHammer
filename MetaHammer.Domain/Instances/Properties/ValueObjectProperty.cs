namespace MetaHammer.Domain.Instances.Properties;

/// <summary>
/// Propiedad tipada que almacena una instancia de objeto de valor.
/// </summary>
/// <param name="propertyName">Nombre de la propiedad.</param>
/// <param name="value">Instancia de ValueObject a almacenar.</param>
public class ValueObjectProperty(string propertyName, ValueObjectInstance value) : PropertyBase(propertyName)
{
    /// <summary>
    /// Instancia de ValueObject almacenada.
    /// </summary>
    public ValueObjectInstance Value { get; } = value;
}
