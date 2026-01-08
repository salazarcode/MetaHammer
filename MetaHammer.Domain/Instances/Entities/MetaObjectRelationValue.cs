using MetaHammer.Domain.Exceptions;
using MetaHammer.Domain.Instances.Base;
using MetaHammer.Domain.Types.Entities;

namespace MetaHammer.Domain.Instances.Entities;

public class MetaObjectRelationValue(MetaRelation definition) : MetaObjectRelationAccess(definition)
{
    private MetaObject? _relatedObject;
    public Guid? RelatedId { get; private set; }

    public override bool IsLoaded
    {
        get => _relatedObject != null; 
        protected set
        {
            // No hace nada, el valor se determina por la presencia del objeto relacionado
        }
    }

    // Conecta un objeto existente
    public void Attach(MetaObject obj)
    {
        _relatedObject = obj;
        RelatedId = obj.Guid;
    }

    // Quita la relación
    public void Detach()
    {
        _relatedObject = null;
        RelatedId = null;
    }

    // Inyecta el objeto desde la persistencia (usado por el Repository)
    public override void Hydrate(IEnumerable<MetaObject> objects)
    {
        _relatedObject = objects.FirstOrDefault();
        RelatedId = _relatedObject?.Guid;
    }

    public override MetaObject GetValue()
    {
        if (!IsLoaded && RelatedId.HasValue)
            throw new DomainException($"La relación '{Name}' no ha sido cargada (Lazy). ID pendiente: {RelatedId}");

        return _relatedObject 
               ?? throw new DomainException($"La relación '{Name}' está vacía.");
    }

    public override IReadOnlyList<MetaObject> GetList() 
        => throw new DomainException($"La relación '{Name}' no es una lista. Use GetValue().");
}