using MetaHammer.Domain.Types.Interfaces;
using MetaHammer.Domain.Types.Methods;

namespace MetaHammer.Domain.Types.Abstract;

public abstract class StructuralType : MetaType, IStructuralType
{
    private List<PropertyDefinition> properties = new();
    
    private List<Method> methods = new();
    
    public IReadOnlyCollection<PropertyDefinition> Properties => properties.AsReadOnly();
    public IReadOnlyCollection<Method> Methods => methods.AsReadOnly();
    public IReadOnlyCollection<Method> GetConstructors() => methods.Where(m => m.IsConstructor).ToList().AsReadOnly();
    
    public void AddProperty(string name, IPropertyType type, bool isArray = false)
    {
        properties.Add(new PropertyDefinition
        {
            Guid = Guid.NewGuid(),
            Name = name,
            IsArray = isArray,
            Type = type
        });
    }
}