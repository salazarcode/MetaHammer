namespace MetaHammer.Domain.Instances.Properties;

/// <summary>
/// Propiedad tipada que almacena un identificador único global (System.Guid).
/// </summary>
/// <param name="propertyName">Nombre de la propiedad.</param>
/// <param name="value">Valor Guid a almacenar.</param>
public class GuidProperty(string propertyName, Guid value) : PropertyBase(propertyName)
{
    /// <summary>
    /// Valor GUID almacenado.
    /// </summary>
    public Guid Value { get; } = value;
}
