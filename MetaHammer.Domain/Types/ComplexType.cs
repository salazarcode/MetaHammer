using MetaHammer.Domain.Types.Interfaces;
using MetaHammer.Domain.Types.Methods;

namespace MetaHammer.Domain.Types;

using MetaHammer.Domain.Types.Abstract;

public class ComplexType : MetaType, ITypeWithProperties
{
    public List<PropertyDefinition> Properties { get; set; } = new();
    public List<RelationDefinition> Relations { get; set; } = new();
    public List<Method> Methods { get; set; } = new();

    public IEnumerable<Method> GetConstructors() => Methods.Where(m => m.IsConstructor);
}