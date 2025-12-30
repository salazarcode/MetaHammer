using MetaHammer.Domain.Types.Interfaces;
using MetaHammer.Domain.Types.Methods;

namespace MetaHammer.Domain.Types.Abstract;

/// <summary>
/// Clase base abstracta para tipos que tienen estructura (propiedades y métodos).
/// Hereda de <see cref="MetaType"/> y es base para <see cref="ComplexType"/> y <see cref="ValueObjectType"/>.
/// </summary>
public abstract class StructuralType : MetaType, IStructuralType
{
    private List<PropertyDefinition> properties = new();
    private List<Method> methods = new();

    /// <summary>
    /// Colección de definiciones de propiedades del tipo.
    /// </summary>
    public IReadOnlyCollection<PropertyDefinition> Properties => properties.AsReadOnly();

    /// <summary>
    /// Colección de métodos definidos en el tipo.
    /// </summary>
    public IReadOnlyCollection<Method> Methods => methods.AsReadOnly();

    /// <summary>
    /// Obtiene los métodos marcados como constructores.
    /// </summary>
    /// <returns>Colección de métodos donde <see cref="Method.IsConstructor"/> es true.</returns>
    public IReadOnlyCollection<Method> GetConstructors() => methods.Where(m => m.IsConstructor).ToList().AsReadOnly();

    /// <summary>
    /// Agrega una definición de propiedad al tipo.
    /// </summary>
    /// <param name="name">Nombre de la propiedad en formato snake_case.</param>
    /// <param name="type">Tipo de la propiedad (primitivo o ValueObject).</param>
    /// <param name="isArray">Indica si la propiedad puede contener múltiples valores.</param>
    public void AddProperty(string name, IPropertyType type, bool isArray = false)
    {
        properties.Add(new PropertyDefinition
        {
            Guid = Guid.NewGuid(),
            Name = name,
            IsArray = isArray,
            Type = type
        });
    }
}