using MetaHammer.Domain.Common;
using MetaHammer.Domain.Types.Interfaces;
using MetaHammer.Domain.Validation;

namespace MetaHammer.Domain.Types.Abstract;

public class PropertyDefinition : Entity
{
    public string Name { get; private set; }
    public IPropertyType Type { get; private set; }
    public bool IsArray { get; private set; }

    public PropertyDefinition(string name, IPropertyType type, bool isArray) : base(System.Guid.NewGuid())
    {
        NameFormatValidator.ValidateSnakeCase(name, "Property");
        Name = name;
        Type = type;
        IsArray = isArray;
    }
}