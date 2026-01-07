using MetaHammer.Domain.Common;
using MetaHammer.Domain.Types.Interfaces;
using MetaHammer.Domain.Validation;

namespace MetaHammer.Domain.Types.Entities;

public class MetaProperty
{ 
    public MetaProperty(string name, IPropertyType metaType, bool isArray = false)
    {
        NameFormatValidator.ValidateSnakeCase(name, "Property");
        Name = name;
        MetaType = metaType;
        IsArray = isArray;
    }
    
    public string Name { get; private set; }
    public IPropertyType MetaType { get; private set; }
    public bool IsArray { get; private set; }
}