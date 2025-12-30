namespace MetaHammer.Domain.Instances.Properties;

/// <summary>
/// Propiedad tipada que almacena un valor de texto (System.String).
/// </summary>
/// <param name="propertyName">Nombre de la propiedad.</param>
/// <param name="value">Valor de texto a almacenar.</param>
public class StringProperty(string propertyName, string value) : PropertyBase(propertyName)
{
    /// <summary>
    /// Valor de texto almacenado.
    /// </summary>
    public string Value { get; } = value;
}
