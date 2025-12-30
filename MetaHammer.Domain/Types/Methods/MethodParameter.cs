using MetaHammer.Domain.Types.Abstract;

namespace MetaHammer.Domain.Types.Methods;

/// <summary>
/// Representa un parámetro de un método.
/// Define el nombre, tipo y posición del parámetro.
/// </summary>
public class MethodParameter
{
    /// <summary>
    /// Identificador único del parámetro.
    /// </summary>
    public Guid Guid { get; set; }

    /// <summary>
    /// Nombre del parámetro.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Tipo del parámetro.
    /// </summary>
    public MetaType Type { get; set; } = null!;

    /// <summary>
    /// Indica si el parámetro es una colección.
    /// </summary>
    public bool IsArray { get; set; } = false;

    /// <summary>
    /// Posición del parámetro en la lista (1-indexed).
    /// </summary>
    public int Order { get; set; }
}