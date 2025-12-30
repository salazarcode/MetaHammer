using MetaHammer.Domain.Validation;

namespace MetaHammer.Domain.Types.Abstract;

public abstract class MetaType
{
    private string _name = string.Empty;

    public Guid Guid { get; protected set; }

    public string Name
    {
        get => _name;
        protected set
        {
            NameFormatValidator.ValidatePascalCase(value, "Type");
            _name = value;
        }
    }

    public int Version { get; protected set; } = 1;
}