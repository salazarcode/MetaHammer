using MetaHammer.Domain.Types.Methods;

namespace MetaHammer.Domain.Types;

using MetaHammer.Domain.Types.Abstract;
using MetaHammer.Domain.Types.Interfaces;

public class ValueObjectType : MetaType, IPropertyType, ITypeWithProperties
{
    public List<PropertyDefinition> Properties { get; set; } = new();
    public List<Method> Methods { get; set; } = new();

    public IEnumerable<Method> GetConstructors() => Methods.Where(m => m.IsConstructor);
}