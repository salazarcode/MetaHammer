using MetaHammer.Domain.Validation;

namespace MetaHammer.Domain.Types.Abstract;

public class RelationDefinition
{
    private string _name = string.Empty;

    public Guid Guid { get; set; }

    public string Name
    {
        get => _name;
        set
        {
            NameFormatValidator.ValidateSnakeCase(value, "Relation");
            _name = value;
        }
    }

    public bool IsArray { get; set; } = false;
    public bool IsComposition { get; set; } = false;
    public ComplexType Type { get; set; } = null!;
}