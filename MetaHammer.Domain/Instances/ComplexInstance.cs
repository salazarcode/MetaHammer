using MetaHammer.Domain.Exceptions;
using MetaHammer.Domain.Instances.Abstract;
using MetaHammer.Domain.Types;
using MetaHammer.Domain.Types.Abstract;

namespace MetaHammer.Domain.Instances;

/// <summary>
/// Instancia de un tipo complejo. Representa una entidad o agregado con identidad propia.
/// Puede tener propiedades y relaciones con otras instancias.
/// </summary>
public class ComplexInstance : MetaInstanceWithProperties
{
    /// <summary>
    /// Lista interna de relaciones de la instancia.
    /// </summary>
    private List<Relation> Relations { get; } = new();

    /// <summary>
    /// Colección de solo lectura de las relaciones de la instancia.
    /// </summary>
    public IReadOnlyCollection<Relation> RelationsAsReadOnly => Relations.AsReadOnly();

    /// <summary>
    /// Crea una nueva instancia de tipo complejo.
    /// </summary>
    /// <param name="guid">Identificador único de la instancia.</param>
    /// <param name="type">Tipo complejo del cual se crea la instancia.</param>
    public ComplexInstance(Guid guid, ComplexType type)
    {
        Guid = guid;
        Type = type;
    }

    /// <summary>
    /// Establece una relación con otra instancia mediante su GUID.
    /// </summary>
    /// <param name="relationName">Nombre de la relación según la definición del tipo.</param>
    /// <param name="instanceGuid">GUID de la instancia destino.</param>
    /// <exception cref="DomainException">Si la relación no existe o ya tiene valor (sin IsArray).</exception>
    public void SetRelation(string relationName, Guid instanceGuid)
    {
        var definition = FindRelationDefinition(relationName);
        ValidateCanAddRelation(relationName, definition);
        Relations.Add(new Relation(relationName, instanceGuid));
    }

    /// <summary>
    /// Busca la definición de una relación en el tipo complejo.
    /// </summary>
    /// <param name="relationName">Nombre de la relación.</param>
    /// <returns>Definición de la relación encontrada.</returns>
    /// <exception cref="DomainException">Si la relación no existe en el tipo.</exception>
    private RelationDefinition FindRelationDefinition(string relationName)
    {
        var complexType = (ComplexType)Type;
        var definition = complexType.Relations.FirstOrDefault(r => r.Name == relationName);

        if (definition == null)
            throw new DomainException($"Relation '{relationName}' does not exist in type '{Type.Name}'");

        return definition;
    }

    /// <summary>
    /// Valida que se pueda agregar una relación.
    /// Si la relación no es un array y ya tiene valor, lanza excepción.
    /// </summary>
    /// <param name="relationName">Nombre de la relación.</param>
    /// <param name="definition">Definición de la relación.</param>
    /// <exception cref="DomainException">Si la relación ya tiene valor y no es array.</exception>
    private void ValidateCanAddRelation(string relationName, RelationDefinition definition)
    {
        if (!definition.IsArray && Relations.Any(r => r.RelationName == relationName))
            throw new DomainException($"Relation '{relationName}' already has a value. Use IsArray=true to allow multiple values.");
    }
}
