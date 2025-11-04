using MetaHammer.Domain.Entities.Abstract;
using MetaHammer.Domain.Entities.Interfaces;

namespace MetaHammer.Domain.Entities.Types;

/// <summary>
/// Representa una Entidad. Es un tipo complejo con una identidad única y ciclo de vida propio.
/// Se materializa como un nodo en el grafo.
/// Implementa IVersionable para permitir que su esquema evolucione.
/// </summary>
public class ComplexType : BaseType, IVersionable
{
    /// <summary>
    /// Lista de otros ComplexType de los que hereda (herencia múltiple).
    /// </summary>
    public List<ComplexType> Parents { get; set; } = new();
        
    /// <summary>
    /// Lista de propiedades internas (Composición).
    /// </summary>
    public List<Property> Properties { get; set; } = new();
        
    /// <summary>
    /// Lista de relaciones externas con otras Entidades (Asociación/Agregación).
    /// </summary>
    public List<Relation> Relations { get; set; } = new();

    // --- Implementación de IVersionable ---
    public int Version { get; set; } = 1;
    public IVersionable? PreviousVersion { get; set; }

    public ComplexType(string name) : base() { Name = name; }
    public ComplexType(Guid guid, string name) : base(guid) { Name = name; }
}