using MetaHammer.Domain.Common;
using MetaHammer.Domain.Types.Entities.Method;
using MetaHammer.Domain.Types.Interfaces;
using MetaHammer.Domain.Validation;

namespace MetaHammer.Domain.Types.Base;

public class MetaType : AggregateRoot
{
    public MetaType(Guid guid, string name) : base(guid)
    {
        NameFormatValidator.ValidatePascalCase(name, "Type");
        Name = name;
        Version = 1;
    }
    #region Identity
    
    public string Name { get; init; }
    
    #endregion

    #region Versioning
    public int Version { get; private set; }
    
    public void SetVersion(int version) => Version = version;
    
    protected void IncrementVersion() => Version++;
    
    #endregion

    #region Methods
    
    private List<Method> _methods = new();
    public IReadOnlyCollection<Method> Methods => _methods.AsReadOnly();

    public IReadOnlyCollection<Method> Constructors() => _methods.Where(m => m.IsConstructor).ToList().AsReadOnly();

    public Method AddMethod(string name, IMetaType? returnType, bool returnsArray = false, bool isStatic = false)
    {
        var method = new Method(this.Guid, name, returnType, returnsArray, isStatic);
        _methods.Add(method);
        return method;
    }
    public Method AddConstructor()
    {
        var method = new Method(this.Guid,"_constructor", null, false, false, true);
        _methods.Add(method);
        return method;
    }
    public Method? GetConstructorBySignature(params ComplexMetaType[] parameterTypes)
    {
        var signature = string.Join("_constructor(", parameterTypes.Select(t => t.Name), ")");
        var constructor = Methods.FirstOrDefault(m => m.GetSignature() == signature);
        return constructor;
    }

    #endregion
}