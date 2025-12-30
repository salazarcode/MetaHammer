namespace MetaHammer.Domain.Instances.Interfaces;

/// <summary>
/// Contrato base para todas las propiedades de una instancia.
/// Define el identificador único y el nombre de la propiedad.
/// </summary>
public interface IProperty
{
    /// <summary>
    /// Identificador único de la propiedad.
    /// </summary>
    Guid Guid { get; }

    /// <summary>
    /// Nombre de la propiedad (corresponde a la definición en el tipo).
    /// </summary>
    string Name { get; }
}
