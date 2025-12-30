using MetaHammer.Domain.Types.Interfaces;
using MetaHammer.Domain.Types.Methods;
using MetaHammer.Domain.Types.Abstract;

namespace MetaHammer.Domain.Types;

public class ComplexType : StructuralType
{
    public ComplexType(string name)
    {
        Guid = Guid.NewGuid();
        Name = name;
    }
    private List<RelationDefinition> relations = new();
    public IReadOnlyCollection<RelationDefinition> Relations => relations.AsReadOnly();

    public void AddRelation(string name, bool isArray, ComplexType type, bool isComposition = false)
    {
        relations.Add(new RelationDefinition
        {
            Guid = Guid.NewGuid(),
            Name = name,
            IsArray = isArray,
            IsComposition = isComposition,
            Type = type
        });
    }
}