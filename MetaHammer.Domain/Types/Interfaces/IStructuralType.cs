using MetaHammer.Domain.Types.Abstract;
using MetaHammer.Domain.Types.Methods;

namespace MetaHammer.Domain.Types.Interfaces;

/// <summary>
/// Contrato para tipos que tienen estructura (propiedades y métodos).
/// Implementado por <see cref="ComplexType"/> y <see cref="ValueObjectType"/>.
/// </summary>
public interface IStructuralType
{
    /// <summary>
    /// Colección de definiciones de propiedades del tipo.
    /// </summary>
    public IReadOnlyCollection<PropertyDefinition> Properties { get; }

    /// <summary>
    /// Colección de métodos definidos en el tipo.
    /// </summary>
    public IReadOnlyCollection<Method> Methods { get; }

    /// <summary>
    /// Agrega una definición de propiedad al tipo.
    /// </summary>
    /// <param name="name">Nombre de la propiedad en formato snake_case.</param>
    /// <param name="type">Tipo de la propiedad (primitivo o ValueObject).</param>
    /// <param name="isArray">Indica si la propiedad puede contener múltiples valores.</param>
    public void AddProperty(string name, IPropertyType type, bool isArray);
}