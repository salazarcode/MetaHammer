using MetaHammer.Domain.Common;
using MetaHammer.Domain.Exceptions;
using MetaHammer.Domain.Types.Entities;
using MetaHammer.Domain.Types.Enums;
using MetaHammer.Domain.Types.Entities.Method;
using MetaHammer.Domain.Validation;

namespace MetaHammer.Domain.Types;

public class MetaType: AggregateRoot
{
    public MetaType(Guid guid, string name, MetaTypeNature nature, bool isNative = false) : base(guid)
    {
        NameFormatValidator.ValidatePascalCase(name, "Type");
        Name = name;
        Version = 1;
        
        if(nature != MetaTypeNature.Primitive && isNative)
            throw new InvalidOperationException("Solo los tipos primitivos pueden ser nativos.");
        
        Nature = nature;
        IsNative = isNative;
    }
    
    public string Name { get; init; }
    public bool IsNative { get; init; }
    public MetaTypeNature Nature { get; init; }

    #region Versioning
    public int Version { get; private set; }
    
    public void SetVersion(int version) => Version = version;
    
    protected void IncrementVersion() => Version++;
    
    #endregion

    #region Methods
    
    private List<Method> _methods = new();
    public IReadOnlyCollection<Method> Methods => _methods.AsReadOnly();

    public IReadOnlyCollection<Method> Constructors() => _methods.Where(m => m.IsConstructor).ToList().AsReadOnly();

    public Method AddMethod(string name, MetaType? returnType, bool returnsArray = false, bool isStatic = false)
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
    public Method? GetConstructorBySignature(params MetaType[] parameterTypes)
    {
        var signature = string.Join("_constructor(", parameterTypes.Select(t => t.Name), ")");
        var constructor = Methods.FirstOrDefault(m => m.GetSignature() == signature);
        return constructor;
    }

    #endregion
    
    #region Properties
    

    private List<MetaProperty> _properties { get; set; } = new();
    
    public IReadOnlyCollection<MetaProperty> Properties => _properties.AsReadOnly();

    public void AddProperty(string name, MetaType propertyType, bool isArray = false, bool isComposition = true)
    {
        switch (this.Nature)
        {
            case MetaTypeNature.Primitive:
                throw new DomainException("Los tipos primitivos no pueden tener propiedades.");
                break;
            case MetaTypeNature.ValueObject:
                if(!(new[]{MetaTypeNature.Primitive, MetaTypeNature.ValueObject}.Contains(propertyType.Nature)))
                {
                    throw new DomainException("Los tipos ValueObject solo pueden tener propiedades de tipos primitivos o de valor.");
                }
                break;
        }

        var property = _properties.FirstOrDefault(p => p.Name == name && p.MetaType.Name == propertyType.Name);
        
        if(property == null)
        {
            _properties.Add(new MetaProperty(this.Guid, name, propertyType, isArray, isComposition));
        }
        else
        {
            throw new DomainException($"La propiedad con nombre '{name}' y tipo {property.MetaType.Name} ya existe en el tipo '{this.Name}'.");
        }
    }
    
    #endregion
}