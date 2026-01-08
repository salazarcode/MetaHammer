using MetaHammer.Domain.Types.Entities;

namespace MetaHammer.Domain.Instances.Base;

public abstract class MetaObjectPropertyAccess(MetaProperty definition)
{
    public MetaProperty Definition { get; } = definition;
    public string Name => Definition.Name;

    
    public abstract void Set(MetaObject value);
    public abstract void Add(MetaObject value);
    
    // Getters específicos (lanzarán excepción si se usan mal)
    public abstract MetaObject GetValue();
    public abstract IReadOnlyList<MetaObject> GetList();
}