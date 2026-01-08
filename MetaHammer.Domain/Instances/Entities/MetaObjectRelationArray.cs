using MetaHammer.Domain.Exceptions;
using MetaHammer.Domain.Instances.Base;
using MetaHammer.Domain.Types.Entities;

namespace MetaHammer.Domain.Instances.Entities;

public class MetaObjectRelationArray(MetaRelation definition) : MetaObjectRelationAccess(definition)
{
    private readonly List<MetaObject> _items = new();

    public override bool IsLoaded { get; protected set; } = true; // Por defecto cargada si se crea vacía

    public void Attach(MetaObject obj)
    {
        if (!_items.Any(i => i.Guid == obj.Guid))
            _items.Add(obj);
    }

    public void Detach(Guid id) => _items.RemoveAll(i => i.Guid == id);

    public override void Hydrate(IEnumerable<MetaObject> objects)
    {
        _items.Clear();
        _items.AddRange(objects);
        IsLoaded = true;
    }
    public override MetaObject GetValue() 
        => throw new DomainException($"La relación '{Name}' es una lista. Use GetList().");

    public override IReadOnlyList<MetaObject> GetList() => _items.AsReadOnly();
}