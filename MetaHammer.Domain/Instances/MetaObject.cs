using MetaHammer.Domain.Types;
using MetaHammer.Domain.Types.Enums;

namespace MetaHammer.Domain.Instances;

public class MetaObject(MetaType type)
{
    public required MetaType MetaType { get; init; } = type;
    private readonly Dictionary<string, MetaObject> _properties = new();
    public IReadOnlyDictionary<string, MetaObject> Properties => _properties;

    public void SetPropertyValue(string name, MetaObject obj)
    {
        switch (this.MetaType.Nature)
        {
            case MetaTypeNature.Primitive:
                throw new Exception($"Primitive types can't have properties.");
            case MetaTypeNature.ValueObject when obj.MetaType.Nature == MetaTypeNature.Complex:
                throw new Exception($"Value objects can't have complex type properties.");
        }
        
        var property = this.MetaType.Properties.FirstOrDefault(x => x.Name == name && x.MetaType.Name == obj.MetaType.Name);
        
        if (property == null)
            throw new Exception($"Property '{name}' of type '{obj.MetaType.Name}' does not exist on type '{this.MetaType.Name}'.");
        
        if(property.IsArray)
            _properties.Add(name, obj);
        else
        {
            _properties[name] = obj;
        }
    }
}