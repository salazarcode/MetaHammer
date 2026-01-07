using MetaHammer.Domain.Common;
using MetaHammer.Domain.Validation;

namespace MetaHammer.Domain.Types.Entities;

public class MetaRelation
{ 
    public MetaRelation(string name, ComplexMetaType complexMetaType, bool isArray = false, bool isComposition = false)
    {
        NameFormatValidator.ValidateSnakeCase(name, "Relation");
        Name = name;
        MetaType = complexMetaType;
        IsArray = isArray;
        IsComposition = isComposition;
    }
    
    public string Name { get; private set; }
    public ComplexMetaType MetaType { get; private set; }
    public bool IsArray { get; private set; }
    public bool IsComposition { get; private set; }
}