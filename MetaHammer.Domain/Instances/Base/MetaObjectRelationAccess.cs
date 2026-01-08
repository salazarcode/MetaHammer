using MetaHammer.Domain.Types.Entities;

namespace MetaHammer.Domain.Instances.Base;

public abstract class MetaObjectRelationAccess(MetaRelation definition)
{
    public MetaRelation Definition { get; } = definition;
    public string Name => Definition.Name;

    // Los "puertos" de consumo (simétricos a PropertyAccess)
    public abstract MetaObject GetValue();
    public abstract IReadOnlyList<MetaObject> GetList();
    public abstract bool IsLoaded { get; protected set; }
    public abstract void Hydrate(IEnumerable<MetaObject> objects);
}