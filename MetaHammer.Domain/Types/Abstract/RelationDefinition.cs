using MetaHammer.Domain.Common;
using MetaHammer.Domain.Validation;

namespace MetaHammer.Domain.Types.Abstract;

public class RelationDefinition : Entity
{
    public string Name { get; private set; }

    public bool IsArray { get; private set; }

    public bool IsComposition { get; private set; }

    public ComplexType Type { get; private set; }

    public RelationDefinition(string name, ComplexType complexType, bool isComposition, bool isArray) : base(System.Guid.NewGuid())
    {
        NameFormatValidator.ValidateSnakeCase(name, "Relation");
        this.Name = name;
        Type = complexType;
        IsComposition = isComposition;
        IsArray = isArray;
    }
}