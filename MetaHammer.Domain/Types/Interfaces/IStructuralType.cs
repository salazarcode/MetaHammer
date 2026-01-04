using MetaHammer.Domain.Types.Abstract;
using MetaHammer.Domain.Types.Methods;

namespace MetaHammer.Domain.Types.Interfaces;

public interface IStructuralType
{
    public IReadOnlyCollection<PropertyDefinition> Properties { get; }

    public IReadOnlyCollection<Method> Methods { get; }

    public void AddProperty(string name, IPropertyType type, bool isArray);
}