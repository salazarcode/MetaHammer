using MetaHammer.Domain.Common;
using MetaHammer.Domain.Validation;

namespace MetaHammer.Domain.Types.Abstract;

/// <summary>
/// Abstract base class for all meta types.
/// Used by types such as PrimitiveType, ValueObjectType, EntityType, and EnumType.
/// </summary>
public abstract class MetaType : AggregateRoot
{
    private string _name = string.Empty;
    public int Version { get; protected set; } = 1;

    public MetaType(Guid guid, string name) : base(guid)
    {
        Name = name;
    }
    
    public string Name
    {
        get => _name;
        protected set
        {
            NameFormatValidator.ValidatePascalCase(value, "Type");
            _name = value;
        }
    }
    
    public void SetVersion(int version) => Version = version;
    protected void IncrementVersion() => Version++;
}