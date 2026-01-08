using MetaHammer.Domain.Exceptions;
using MetaHammer.Domain.Instances.Base;
using MetaHammer.Domain.Instances.Entities;
using MetaHammer.Domain.Types;
using MetaHammer.Domain.Types.Base;
using MetaHammer.Domain.Types.Interfaces;

namespace MetaHammer.Domain.Instances;

public class MetaObject
{
    #region Constructors
    public MetaObject(PrimitiveType type, Object value)
    {
        MetaType = (IMetaType)type;
        Value = value;
    }
    public MetaObject(ValueObjectType type)
    {
        MetaType = (IMetaType)type;
        InitializeContainers(type);
    }
    public MetaObject(ComplexType type)
    {
        MetaType = (IMetaType)type;
        InitializeContainers(type);
    }

    public MetaObject(ComplexType type, Guid guid)
    {
        MetaType = (IMetaType)type;
        Guid = guid;
        IsLoaded = false;
        InitializeContainers(type);
    }
    #endregion

    public Guid? Guid { get; init; } = null;
    public bool IsLoaded { get; set; } = true;
    public IMetaType MetaType { get; private set; }
    private object? _value;
    
    private readonly Dictionary<string, MetaObjectPropertyAccess> _properties = new();
    public IReadOnlyDictionary<string, MetaObjectPropertyAccess> GetProperties => _properties.AsReadOnly();
    
    private readonly Dictionary<string, MetaObjectRelationAccess> _relations = new();
    public IReadOnlyDictionary<string, MetaObjectRelationAccess> GetRelations => _relations.AsReadOnly();
    
    public MetaObjectPropertyAccess Property(string name)
    {
        if (!_properties.TryGetValue(name, out var prop))
            throw new DomainException($"La propiedad '{name}' no existe en el tipo '{MetaType.Name}'.");
        return prop;
    }

    public MetaObjectRelationAccess Relation(string name)
    {
        if (!_relations.TryGetValue(name, out var rel))
            throw new DomainException($"La relación '{name}' no existe en el tipo '{MetaType.Name}'.");
        return rel;
    }
    private void InitializeContainers(MetaTypeWithProperties type)
    {
        foreach (var prop in type.Properties)
        {
            _properties[prop.Name] = prop.IsArray 
                ? new MetaObjectPropertyArray(prop) 
                : new MetaObjectPropertyValue(prop);
        }
    }
    public Object? Value {
        get => this.MetaType is not PrimitiveType ? null : _value;
        set
        {
            if(this.MetaType is not PrimitiveType)    
                throw new Exception("Solo los objetos de tipo primitivo tienen valor.");
            _value = value;
        }
    }
}