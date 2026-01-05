using MetaHammer.Domain.Common;
using MetaHammer.Domain.Exceptions;
using MetaHammer.Domain.Types.Enums;
using MetaHammer.Domain.Types.Methods;
using MetaHammer.Domain.Validation;

namespace MetaHammer.Domain.Types;

public class MetaType: AggregateRoot
{
    protected MetaType(Guid guid, string name, MetaTypeNature category, bool isNative = false) : base(guid)
    {
        NameFormatValidator.ValidatePascalCase(name, "Type");
        Name = name;
        Version = 1;
        Category = category;
        
        if(category != MetaTypeNature.Primitive && isNative)
            throw new InvalidOperationException("Solo los tipos primitivos pueden ser nativos.");
        
        IsNative = isNative;
    }
    
    public string Name { get; init; }
    public bool IsNative { get; init; }
    public MetaTypeNature Category { get; init; }

    #region Versioning
    public int Version { get; private set; }
    
    public void SetVersion(int version) => Version = version;
    
    protected void IncrementVersion() => Version++;
    
    #endregion

    #region Methods
    
    private List<Method> _methods = new();
    public IReadOnlyCollection<Method> Methods => _methods.AsReadOnly();

    public IReadOnlyCollection<Method> Constructors() => _methods.Where(m => m.IsConstructor).ToList().AsReadOnly();

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
    public Method? GetConstructorBySignature(params MetaType[] parameterTypes)
    {
        var signature = string.Join("_constructor(", parameterTypes.Select(t => t.Name), ")");
        var constructor = Methods.FirstOrDefault(m => m.GetSignature() == signature);
        return constructor;
    }

    #endregion
    
    #region Properties
    

    private List<Property> _properties { get; set; } = new();
    
    public IReadOnlyCollection<Property> Properties => _properties.AsReadOnly();

    public void AddProperty(string name, MetaType propertyType, bool isArray = false, bool isComposition = true)
    {
        switch (this.Category)
        {
            case MetaTypeNature.Primitive:
                throw new DomainException("Los tipos primitivos no pueden tener propiedades.");
                break;
            case MetaTypeNature.ValueObject:
                if(!(new[]{MetaTypeNature.Primitive, MetaTypeNature.ValueObject}.Contains(propertyType.Category)))
                {
                    throw new DomainException("Los tipos ValueObject solo pueden tener propiedades de tipos primitivos o de valor.");
                }
                break;
        }

        var property = _properties.FirstOrDefault(p => p.Name == name && p.MetaType.Name == propertyType.Name);
        
        if(property != null)
        {
            throw new DomainException($"La propiedad con nombre '{name}' y tipo {property.MetaType.Name} ya existe en el tipo '{this.Name}'.");
        }
        else
        {
            _properties.Add(property!);
        }
    }
    
    #endregion
}