using MetaHammer.Domain.Exceptions;
using MetaHammer.Domain.Instances.Abstract;
using MetaHammer.Domain.Types;
using MetaHammer.Domain.Types.Abstract;

namespace MetaHammer.Domain.Instances;

public class ComplexInstance : StructuralInstance
{
    private List<Relation> Relations { get; } = new();

    public IReadOnlyCollection<Relation> RelationsAsReadOnly => Relations.AsReadOnly();

    public ComplexInstance(Guid guid, ComplexType type) : base(guid, type)
    {
        Guid = guid;
        Type = type;
    }

    public void SetRelation(string relationName, Guid instanceGuid)
    {
        var definition = FindRelationDefinition(relationName);
        ValidateCanAddRelation(relationName, definition);
        Relations.Add(new Relation(relationName, instanceGuid));
    }

    private RelationDefinition FindRelationDefinition(string relationName)
    {
        var complexType = (ComplexType)Type;
        var definition = complexType.Relations.FirstOrDefault(r => r.Name == relationName);

        if (definition == null)
            throw new DomainException($"Relation '{relationName}' does not exist in type '{Type.Name}'");

        return definition;
    }

    private void ValidateCanAddRelation(string relationName, RelationDefinition definition)
    {
        if (!definition.IsArray && Relations.Any(r => r.RelationName == relationName))
            throw new DomainException($"Relation '{relationName}' already has a value. Use IsArray=true to allow multiple values.");
    }
}
