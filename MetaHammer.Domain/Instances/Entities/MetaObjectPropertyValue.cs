using MetaHammer.Domain.Exceptions;
using MetaHammer.Domain.Instances.Base;
using MetaHammer.Domain.Types.Entities;

namespace MetaHammer.Domain.Instances.Entities;

public class MetaObjectPropertyValue(MetaProperty definition) : MetaObjectPropertyAccess(definition)
{
    private MetaObject? _value;

    public override void Set(MetaObject value)
    {
        // Validación de seguridad de tipos
        if (value.MetaType.Guid != Definition.MetaType.Guid)
            throw new DomainException($"Tipo inválido. Se esperaba {Definition.MetaType.Name}.");

        _value = value;
    }

    public override void Add(MetaObject value)
    {
        throw new DomainException($"La propiedad {Name} no es una lista.");
    }

    public override MetaObject GetValue() => _value 
                                             ?? throw new DomainException($"La propiedad {Name} no ha sido inicializada.");

    public override IReadOnlyList<MetaObject> GetList() 
        => throw new DomainException($"La propiedad {Name} no es una lista.");
}