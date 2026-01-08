using MetaHammer.Domain.Exceptions;
using MetaHammer.Domain.Instances.Base;
using MetaHammer.Domain.Types.Entities;

namespace MetaHammer.Domain.Instances.Entities;

public class MetaObjectPropertyArray(MetaProperty definition) : MetaObjectPropertyAccess(definition)
{
    private readonly List<MetaObject> _items = new();

    public override void Set(MetaObject value)
    {
        throw new DomainException($"La propiedad {Name} es una lista. Use Add().");
    }

    public override void Add(MetaObject value)
    {
        if (value.MetaType.Guid != Definition.MetaType.Guid)
            throw new DomainException($"Tipo inválido para la lista {Name}.");

        _items.Add(value);
    }

    public override MetaObject GetValue() 
        => throw new DomainException($"La propiedad {Name} es una lista. Use GetList().");

    public override IReadOnlyList<MetaObject> GetList() => _items.AsReadOnly();
}