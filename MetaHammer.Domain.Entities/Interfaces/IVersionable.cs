namespace MetaHammer.Domain.Entities.Interfaces;

/// <summary>
/// Define el contrato para los tipos que soportan versionado.
/// Esto permite que el metamodelo evolucione de forma segura y rastreable.
/// </summary>
public interface IVersionable
{
    /// <summary>
    /// El número de versión de esta definición de tipo.
    /// </summary>
    int Version { get; set; }

    /// <summary>
    /// Un enlace (opcional) a la versión anterior de este tipo.
    /// Es 'null' si esta es la primera versión (v1).
    /// </summary>
    IVersionable? PreviousVersion { get; set; }
}