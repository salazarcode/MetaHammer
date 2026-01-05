using MetaHammer.Domain.Common;
using MetaHammer.Domain.Validation;

namespace MetaHammer.Domain.Types;

public class Property : Entity
{ 
    public Property(Guid guid, string name, MetaType metaType, bool isArray = false, bool isComposition = false) : base(guid)
    {
        NameFormatValidator.ValidateSnakeCase(name, "Property");
        Name = name;
        MetaType = metaType;
        IsArray = isArray;
        IsComposition = isComposition;
    }
    
    public string Name { get; private set; }
    public MetaType MetaType { get; private set; }
    public bool IsArray { get; private set; }
    public bool IsComposition { get; private set; }
}