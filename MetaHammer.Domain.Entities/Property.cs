using MetaHammer.Domain.Entities.Interfaces;

namespace MetaHammer.Domain.Entities;

/// <summary>
/// Define una propiedad intrínseca de un ComplexType o ValueObjectType.
/// Representa una relación de Composición.
/// </summary>
public class Property
{
    /// <summary>
    /// Nombre de la propiedad (ej. "Name", "Age", "Address").
    /// </summary>
    public string Name { get; set; }
        
    /// <summary>
    /// El tipo de la propiedad, restringido a ser un PrimitiveType o ValueObjectType.
    /// </summary>
    public IPropertyType Type { get; set; }
        
    /// <summary>
    /// Indica si esta propiedad es una colección (ej. una lista de "Tags").
    /// </summary>
    public bool IsArray { get; set; }

    public Property(string name, IPropertyType type, bool isArray = false)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre de la propiedad no puede estar vacío.", nameof(name));
                
        Name = name;
        Type = type ?? throw new ArgumentNullException(nameof(type));
        IsArray = isArray;
    }
}