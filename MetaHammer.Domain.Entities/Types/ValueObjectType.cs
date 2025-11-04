using MetaHammer.Domain.Entities.Abstract;
using MetaHammer.Domain.Entities.Interfaces;

namespace MetaHammer.Domain.Entities.Types;

/// <summary>
/// Representa un Objeto de Valor (VO). Es un tipo complejo definido por sus
/// atributos, no por su identidad.
/// Implementa IPropertyType (puede ser una propiedad) e IVersionable (su
/// estructura puede evolucionar).
/// </summary>
public class ValueObjectType : BaseType, IVersionable, IPropertyType
{
    /// <summary>
    /// Lista de VOs de los que hereda propiedades.
    /// </summary>
    public List<ValueObjectType> Parents { get; set; } = new();

    /// <summary>
    /// Lista de propiedades que definen la estructura de este VO.
    /// </summary>
    public List<IPropertyType> Properties { get; set; } = new();
        
    // --- Implementación de IVersionable ---
    public int Version { get; set; } = 1;
    public IVersionable? PreviousVersion { get; set; }

    public ValueObjectType(string name) : base() { Name = name; }
    public ValueObjectType(Guid guid, string name) : base(guid) { Name = name; }
}