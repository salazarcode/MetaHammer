using MetaHammer.Domain.Types;

namespace MetaHammer.Domain.Instances;

public class Relation(string relationName, Guid instanceGuid)
{
    public Guid Guid { get; private set; } = Guid.NewGuid();

    public string RelationName { get; set; } = relationName;

    public Guid InstanceGuid { get; private set; } = instanceGuid;
}