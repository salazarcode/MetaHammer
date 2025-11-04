using MetaHammer.Domain.Entities.Types;

namespace MetaHammer.Domain.Entities;

/// <summary>
/// Define una relación entre dos ComplexType (Entidades).
/// Representa una Asociación o Agregación.
/// </summary>
public class Relation
{
    /// <summary>
    /// Nombre de la relación (ej. "WorksOn", "ManagedBy").
    /// </summary>
    public string Name { get; set; }
        
    /// <summary>
    /// El tipo de la entidad de destino de la relación.
    /// </summary>
    public ComplexType Type { get; set; }
        
    /// <summary>
    /// Indica si es una relación "a muchos" (ej. un Manager "Managed" muchos Employees).
    /// </summary>
    public bool IsArray { get; set; }
        
    /// <summary>
    /// Si es true, indica Composición (ciclo de vida dependiente).
    /// Si es false, indica Agregación (ciclo de vida independiente).
    /// </summary>
    public bool IsComposition { get; set; }

    /// <summary>
    /// Lista de propiedades que pertenecen a la relación misma
    /// (ej. la relación "WorksOn" tiene una propiedad "Role").
    /// </summary>
    public List<Property> Properties { get; set; } = new();

    public Relation(string name, ComplexType complexType, bool isArray = false, List<Property>? properties = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre de la relación no puede estar vacío.", nameof(name));

        Name = name;
        Type = complexType ?? throw new ArgumentNullException(nameof(complexType));
        IsArray = isArray;
        Properties = properties ?? new();
    }
}