using MetaHammer.Domain.Common;
using MetaHammer.Domain.Validation;

namespace MetaHammer.Domain.Types.Entities;

public class MetaRelation
{ 
    public MetaRelation(string name, ComplexType complexType, bool isArray = false, bool isComposition = false)
    {
        NameFormatValidator.ValidateSnakeCase(name, "Relation");
        Name = name;
        Type = complexType;
        IsArray = isArray;
        IsComposition = isComposition;
    }
    
    public string Name { get; private set; }
    public ComplexType Type { get; private set; }
    public bool IsArray { get; private set; }
    public bool IsComposition { get; private set; }
}