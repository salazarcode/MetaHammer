namespace MetaHammer.Domain.Instances.Properties;

/// <summary>
/// Propiedad tipada que almacena un valor decimal de alta precisión (System.Decimal).
/// </summary>
/// <param name="propertyName">Nombre de la propiedad.</param>
/// <param name="value">Valor decimal a almacenar.</param>
public class DecimalProperty(string propertyName, decimal value) : PropertyBase(propertyName)
{
    /// <summary>
    /// Valor decimal almacenado.
    /// </summary>
    public decimal Value { get; } = value;
}
