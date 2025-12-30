using MetaHammer.Domain.Validation;

namespace MetaHammer.Domain.Types.Abstract;

/// <summary>
/// Define una relación entre tipos complejos.
/// Especifica el tipo destino, cardinalidad y si es una composición.
/// </summary>
public class RelationDefinition
{
    private string _name = string.Empty;

    /// <summary>
    /// Identificador único de la definición de relación.
    /// </summary>
    public Guid Guid { get; set; }

    /// <summary>
    /// Nombre de la relación en formato snake_case (ej: "has_orders", "belongs_to").
    /// </summary>
    public string Name
    {
        get => _name;
        set
        {
            NameFormatValidator.ValidateSnakeCase(value, "Relation");
            _name = value;
        }
    }

    /// <summary>
    /// Indica si la relación puede apuntar a múltiples instancias (uno a muchos).
    /// </summary>
    public bool IsArray { get; set; } = false;

    /// <summary>
    /// Indica si es una composición (el hijo no puede existir sin el padre).
    /// En caso de eliminación del padre, los hijos se eliminan en cascada.
    /// </summary>
    public bool IsComposition { get; set; } = false;

    /// <summary>
    /// Tipo complejo destino de la relación.
    /// </summary>
    public ComplexType Type { get; set; } = null!;
}