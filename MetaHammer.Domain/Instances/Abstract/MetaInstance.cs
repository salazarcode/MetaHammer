using MetaHammer.Domain.Types.Abstract;

namespace MetaHammer.Domain.Instances.Abstract;

/// <summary>
/// Clase base abstracta para todas las instancias del sistema.
/// Una instancia es un objeto concreto creado a partir de un meta-tipo.
/// </summary>
public abstract class MetaInstance
{
    /// <summary>
    /// Identificador único de la instancia.
    /// </summary>
    public Guid Guid { get; protected set; } = Guid.NewGuid();

    /// <summary>
    /// Meta-tipo del cual se creó esta instancia.
    /// </summary>
    public MetaType Type { get; init; }
}
