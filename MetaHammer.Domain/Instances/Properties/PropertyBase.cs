using MetaHammer.Domain.Instances.Interfaces;

namespace MetaHammer.Domain.Instances.Properties;

/// <summary>
/// Clase base abstracta para todas las propiedades tipadas.
/// Proporciona el identificador único y el nombre de la propiedad.
/// </summary>
public abstract class PropertyBase : IProperty
{
    /// <summary>
    /// Identificador único de la propiedad.
    /// </summary>
    public Guid Guid { get; } = Guid.NewGuid();

    /// <summary>
    /// Nombre de la propiedad.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Constructor protegido para clases derivadas.
    /// </summary>
    /// <param name="name">Nombre de la propiedad.</param>
    protected PropertyBase(string name)
    {
        Name = name;
    }
}
