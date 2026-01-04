using MetaHammer.Domain.Types.Interfaces;
using MetaHammer.Domain.Types.Methods;

namespace MetaHammer.Domain.Types.Abstract;

/// <summary>
/// Abstract base class for all structural types (e.g., classes, structs).
/// It includes ValueObjectsTypes and ComplexTypes.
/// It has properties and methods.
/// </summary>
public abstract class StructuralType : MetaType, IStructuralType
{
    private List<PropertyDefinition> _properties = new();
    private List<Method> _methods = new();

    public IReadOnlyCollection<PropertyDefinition> Properties => _properties.AsReadOnly();

    public IReadOnlyCollection<Method> Methods => _methods.AsReadOnly();

    public IReadOnlyCollection<Method> Constructors() => _methods.Where(m => m.IsConstructor).ToList().AsReadOnly();

    public StructuralType(Guid guid, string name) : base(guid, name)
    {
    }
    public void AddProperty(string name, IPropertyType type, bool isArray = false)
    {
        _properties.Add(new PropertyDefinition(name, type, isArray));
    }

    protected Method AddMethod(string name, MetaType? returnType, bool returnsArray = false, bool isStatic = false)
    {
        var method = new Method(name, returnType, returnsArray, isStatic);
        _methods.Add(method);
        return method;
    }

    protected Method AddConstructor()
    {
        var method = new Method("_constructor", null, false, false, true);
        _methods.Add(method);
        return method;
    } 
}