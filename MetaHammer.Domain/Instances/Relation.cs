using MetaHammer.Domain.Types;

namespace MetaHammer.Domain.Instances;

public class Relation(RelationDefinition relationDefinition, Guid instanceGuid)
{
    public Guid Guid { get; private set; } = Guid.NewGuid();
    public RelationDefinition RelationDefinition { get; private set; } = relationDefinition;
    public Guid InstanceGuid { get; private set; } = instanceGuid;
}