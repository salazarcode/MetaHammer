using MetaHammer.Domain.Types;

namespace MetaHammer.Domain.Instances.Base;

public abstract class Instance(string name, MetaType type)
{
    public string Name { get; init; } = name;
    public required MetaType MetaType { get; init; } = type;
    
    private readonly Dictionary<string, Instance> _properties = new();
    
    public IReadOnlyDictionary<string, Instance> Properties => _properties;

    public void SetPropertyValue(string name, Instance obj)
    {
        // Validate that the property exists on the MetaType and that the type matches
        
        switch(type.Name)
        {
            case "PrimitiveType":
            case "EnumType":
            case "ComplexType":
            case "ValueObjectType":
                break;
            default:
                throw new Exception($"Type '{obj.MetaType.Name}' is not a valid property type.");
        }
        
        var thisType = this.MetaType;
        var property = this.MetaType.Properties.FirstOrDefault(x => x.Name == name && x.MetaType.Name == obj.MetaType.Name);
        
        if (property == null)
        {
            throw new Exception($"Property '{name}' of type '{obj.MetaType.Name}' does not exist on type '{thisType.Name}'.");
        }
        else
        {
            if(property.IsArray)
                _properties.Add(name, obj);
            else
            {
                _properties[name] = obj;
            }
        }
    }
}