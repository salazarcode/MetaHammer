namespace MetaHammer.Domain.Instances.Properties;

/// <summary>
/// Propiedad tipada que almacena un valor booleano (System.Boolean).
/// </summary>
/// <param name="propertyName">Nombre de la propiedad.</param>
/// <param name="value">Valor booleano a almacenar.</param>
public class BoolProperty(string propertyName, bool value) : PropertyBase(propertyName)
{
    /// <summary>
    /// Valor booleano almacenado.
    /// </summary>
    public bool Value { get; } = value;
}
