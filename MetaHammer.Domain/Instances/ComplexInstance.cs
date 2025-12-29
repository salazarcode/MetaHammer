using MetaHammer.Domain.Exceptions;
using MetaHammer.Domain.Instances.Abstract;
using MetaHammer.Domain.Types;

namespace MetaHammer.Domain.Instances;

public class ComplexInstance : MetaInstanceWithProperties
{
    public List<Relation> Relations { get; private set; } = new();

    public ComplexInstance(ComplexType type)
    {
        Guid = Guid.NewGuid();
        Type = type;
    }

    public void SetRelation(RelationDefinition relationDefinition, Guid instanceGuid)
    {
        if (!((ComplexType)Type).Relations.Contains(relationDefinition))
        {
            throw new DomainException($"RelationDefinition '{relationDefinition.Name}' does not belong to type '{Type.Name}'");
        }
        Relations.Add(new Relation(relationDefinition, instanceGuid));
    }
}
