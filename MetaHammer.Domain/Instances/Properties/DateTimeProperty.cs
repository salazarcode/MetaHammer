namespace MetaHammer.Domain.Instances.Properties;

/// <summary>
/// Propiedad tipada que almacena un valor de fecha y hora (System.DateTime).
/// </summary>
/// <param name="propertyName">Nombre de la propiedad.</param>
/// <param name="value">Valor DateTime a almacenar.</param>
public class DateTimeProperty(string propertyName, DateTime value) : PropertyBase(propertyName)
{
    /// <summary>
    /// Valor de fecha y hora almacenado.
    /// </summary>
    public DateTime Value { get; } = value;
}
