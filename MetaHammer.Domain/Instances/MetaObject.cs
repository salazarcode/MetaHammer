using MetaHammer.Domain.Exceptions;
using MetaHammer.Domain.Types;
using MetaHammer.Domain.Types.Base;
using MetaHammer.Domain.Types.Interfaces;

namespace MetaHammer.Domain.Instances;

public class MetaObject
{
    public MetaObject(PrimitiveMetaType type, Object value)
    {
        MetaType = (IMetaType)type;
        Value = value;
    }
    public MetaObject(ValueObjectMetaType type)
    {
        MetaType = (IMetaType)type;
    }

    public MetaObject(ComplexMetaType type)
    {
        MetaType = (IMetaType)type;
    }

    public MetaObject(ComplexMetaType type, Guid guid)
    {
        MetaType = (IMetaType)type;
        Guid = guid;
        IsLoaded = false;
    }

    public Guid? Guid { get; init; } = null;
    
    public bool IsLoaded { get; set; } = true;
    public IMetaType MetaType { get; private set; }
    
    private readonly Dictionary<string, Object> _properties = new();
    public IReadOnlyDictionary<string, Object> Properties => _properties.AsReadOnly();
    
    private readonly Dictionary<string, Object> _relations = new();
    public IReadOnlyDictionary<string, Object> Relations => _relations.AsReadOnly();

    public void SetPropertyValue(string name, MetaObject value)
    {
        if (this.MetaType is not MetaTypeWithProperties)
            throw new DomainException($"El tipo '{this.MetaType.Name}' no tiene propiedades.");
        
        var metaTypeWithProperties = (MetaTypeWithProperties)this.MetaType;
        var foundProperty = metaTypeWithProperties.Properties.FirstOrDefault(p => p.Name == name && p.MetaType.Name == value.MetaType.Name);
        
        if(foundProperty == null)
            throw new DomainException($"La propiedad '{name}' de tipo '{value.MetaType.Name}' no existe en el tipo '{this.MetaType.Name}'.");
        
        if (foundProperty.IsArray)
            throw new DomainException($"El tipo '{name}' no es una lista. Use la función 'AddItemToProperty' para agregar elementos a la lista.");
        
        _properties[name] = value;
    }
    
    public void AddItemToProperty(string name, MetaObject value)
    {
        if (this.MetaType is not MetaTypeWithProperties)
            throw new DomainException($"El tipo '{this.MetaType.Name}' no tiene propiedades.");
        
        var metaTypeWithProperties = (MetaTypeWithProperties)this.MetaType;
        var foundProperty = metaTypeWithProperties.Properties.FirstOrDefault(p => p.Name == name && p.MetaType.Name == value.MetaType.Name);
        
        if(foundProperty == null)
            throw new DomainException($"La propiedad '{name}' de tipo '{value.MetaType.Name}' no existe en el tipo '{this.MetaType.Name}'.");
        
        if (!foundProperty.IsArray)
            throw new DomainException($"La propiedad '{name}' no es una lista. Use la función 'SetPropertyValue' para establecer su valor.");
        
        if (!_properties.TryGetValue(name, out var listObj) || listObj is not List<MetaObject> list)
        {
            list = new List<MetaObject>();
            _properties[name] = list;
        }
        list.Add(value);
    }
    
    public void SetRelationValue(string name, MetaObject value)
    {
        if(this.MetaType is not ComplexMetaType)
            throw new DomainException($"El tipo '{this.MetaType.Name}' no tiene relaciones.");
        
        var complexMetaType = (ComplexMetaType)this.MetaType;
        var relation = complexMetaType.Relations.FirstOrDefault(r => r.Name == name && r.MetaType.Name == value.MetaType.Name);
        
        if(relation == null)
            throw new DomainException($"La relación '{name}' de tipo '{value.MetaType.Name}' no existe en el tipo '{this.MetaType.Name}'.");
        
        if(relation.IsArray)
            throw new DomainException($"La relación '{name}' es una lista. Use la función 'AddItemToRelation' para agregar elementos a la lista.");
        
        _relations[name] = value;
    }
    
    public void AddItemToRelation(string name, MetaObject value)
    {
        if(this.MetaType is not ComplexMetaType)
            throw new DomainException($"El tipo '{this.MetaType.Name}' no tiene relaciones.");
        
        var complexMetaType = (ComplexMetaType)this.MetaType;
        var relation = complexMetaType.Relations.FirstOrDefault(r => r.Name == name && r.MetaType.Guid == value.MetaType.Guid);
        
        if(relation == null)
            throw new DomainException($"La relación '{name}' de tipo '{value.MetaType.Name}' no existe en el tipo '{this.MetaType.Name}'.");
        
        if(!relation.IsArray)
            throw new DomainException($"La relación '{name}' no es una lista. Use la función 'SetRelationValue' para establecer su valor.");
        
        if (!_relations.TryGetValue(name, out var listObj) || listObj is not List<MetaObject> list)
        {
            list = new List<MetaObject>();
            _relations[name] = list;
        }
        list.Add(value);
    }
    
    private object? _value;

    public Object? Value {
        get => this.MetaType is not PrimitiveMetaType ? null : _value;
        set
        {
            if(this.MetaType is not PrimitiveMetaType)    
                throw new Exception("Solo los objetos de tipo primitivo tienen valor.");
            _value = value;
        }
    }

    public List<MetaObject> GetPropertyList(string name)
    {
        if(!_properties.TryGetValue(name, out var value))
            throw new DomainException($"La propiedad '{name}' no existe en el objeto de tipo '{this.MetaType.Name}'.");
        return (List<MetaObject>)value;
    }

    public MetaObject Property(string name)
    {
        if(!_properties.TryGetValue(name, out var value))
            throw new DomainException($"La propiedad '{name}' no existe en el objeto de tipo '{this.MetaType.Name}'.");
        return (MetaObject)value;
    }
}