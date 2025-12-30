using MetaHammer.Domain.Types;

namespace MetaHammer.Domain.Instances;

/// <summary>
/// Representa una relación entre una instancia compleja y otra instancia destino.
/// Almacena la referencia mediante el GUID de la instancia destino.
/// </summary>
/// <param name="relationName">Nombre de la relación según la definición del tipo.</param>
/// <param name="instanceGuid">GUID de la instancia destino de la relación.</param>
public class Relation(string relationName, Guid instanceGuid)
{
    /// <summary>
    /// Identificador único de esta relación.
    /// </summary>
    public Guid Guid { get; private set; } = Guid.NewGuid();

    /// <summary>
    /// Nombre de la relación (corresponde a la definición en el tipo).
    /// </summary>
    public string RelationName { get; set; } = relationName;

    /// <summary>
    /// GUID de la instancia destino de la relación.
    /// </summary>
    public Guid InstanceGuid { get; private set; } = instanceGuid;
}