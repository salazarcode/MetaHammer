namespace MetaHammer.Domain.Instances.Properties;

/// <summary>
/// Propiedad tipada que almacena un valor entero (System.Int32).
/// </summary>
/// <param name="propertyName">Nombre de la propiedad.</param>
/// <param name="value">Valor entero a almacenar.</param>
public class IntProperty(string propertyName, int value) : PropertyBase(propertyName)
{
    /// <summary>
    /// Valor entero almacenado.
    /// </summary>
    public int Value { get; } = value;
}
