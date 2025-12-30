using MetaHammer.Domain.Types.Interfaces;
using MetaHammer.Domain.Validation;

namespace MetaHammer.Domain.Types.Abstract;

/// <summary>
/// Define la estructura de una propiedad dentro de un tipo estructural.
/// Especifica el nombre, tipo y si puede contener múltiples valores.
/// </summary>
public class PropertyDefinition
{
    private string _name = string.Empty;

    /// <summary>
    /// Identificador único de la definición de propiedad.
    /// </summary>
    public Guid Guid { get; set; }

    /// <summary>
    /// Nombre de la propiedad en formato snake_case (ej: "first_name", "order_date").
    /// </summary>
    public string Name
    {
        get => _name;
        set
        {
            NameFormatValidator.ValidateSnakeCase(value, "Property");
            _name = value;
        }
    }

    /// <summary>
    /// Indica si la propiedad puede contener múltiples valores (colección).
    /// </summary>
    public bool IsArray { get; set; } = false;

    /// <summary>
    /// Tipo de la propiedad. Puede ser un <see cref="PrimitiveType"/> o <see cref="ValueObjectType"/>.
    /// </summary>
    public IPropertyType Type { get; set; } = null!;
}