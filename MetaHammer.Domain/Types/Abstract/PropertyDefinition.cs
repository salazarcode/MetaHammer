using MetaHammer.Domain.Types.Interfaces;
using MetaHammer.Domain.Validation;

namespace MetaHammer.Domain.Types.Abstract;

public class PropertyDefinition
{
    private string _name = string.Empty;

    public Guid Guid { get; set; }

    public string Name
    {
        get => _name;
        set
        {
            NameFormatValidator.ValidateSnakeCase(value, "Property");
            _name = value;
        }
    }

    public bool IsArray { get; set; } = false;
    public IPropertyType Type { get; set; } = null!;
}