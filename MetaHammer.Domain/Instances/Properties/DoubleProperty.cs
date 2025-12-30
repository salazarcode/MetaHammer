namespace MetaHammer.Domain.Instances.Properties;

/// <summary>
/// Propiedad tipada que almacena un valor de punto flotante doble (System.Double).
/// </summary>
/// <param name="propertyName">Nombre de la propiedad.</param>
/// <param name="value">Valor double a almacenar.</param>
public class DoubleProperty(string propertyName, double value) : PropertyBase(propertyName)
{
    /// <summary>
    /// Valor double almacenado.
    /// </summary>
    public double Value { get; } = value;
}
