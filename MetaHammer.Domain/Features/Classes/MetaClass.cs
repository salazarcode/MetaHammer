using MetaHammer.Domain.Common;
using MetaHammer.Domain.Exceptions;
using MetaHammer.Domain.Features.Classes.Enums;
using MetaHammer.Domain.Features.Classes.Methods;
using MetaHammer.Domain.Features.Organizations.Identity;
using MetaHammer.Domain.Features.Organizations;
using MetaHammer.Domain.Validation;

namespace MetaHammer.Domain.Features.Classes;

/// <summary>
/// Tipo base que contiene cualquier clase.
/// </summary>
public class MetaClass : AggregateRoot
{
    public MetaClass(Guid guid, string name, MetaNature metaNature, Organization organization, User user, bool isNative = false, MetaClass? parentClass = null, List<MetaClass>? interfaces = null) : base(guid)
    {
        NameFormatValidator.ValidatePascalCase(name, "Type");
        Name = name;
        Version = 1;
        MetaNature = metaNature;
        IsNative = isNative;
        
        Organization = organization;
// ... existing code ...
        AddDomainEvent(new Events.MetaClassCreated(Guid, Name, MetaNature, OrganizationGuid, IsNative));
    }
    
    #region Identity
    
    public string Name { get; init; }

    public MetaNature MetaNature { get; set; }

    public bool IsNative { get; init; }

    public MetaClass? ParentClass { get; set; }

    public Guid ParentClassGuid { get; set; }
    
    public List<MetaClass> Interfaces { get; set; } = new();

    //Un tipo le pertenece a un organization
    public Guid OrganizationGuid { get; private set; }
    
    //Un tipo tiene una propiedad lazyloading para el Organization
    private Organization Organization { get; init; }
    
    //Un tipo es creado por un usuario
    public Guid CreatedByGuid { get; private set; }
    
    //Un tipo tiene una propiedad lazyloading para el usuario creador
    private User CreatedBy { get; init; }

    #endregion

    #region Versioning
    public int Version { get; private set; }
    
    public void SetVersion(int version) => Version = version;
    
    protected void IncrementVersion() => Version++;
    
    #endregion

    #region Methods
    
    private List<MetaMethod> Methods { get; set; } = new();
    public IReadOnlyCollection<MetaMethod> GetMethods => Methods.AsReadOnly();

    public IReadOnlyCollection<MetaMethod> Constructors() => Methods.Where(m => m.IsConstructor).ToList().AsReadOnly();

    public MetaMethod AddMethod(string name, MetaClass? returnClass, bool returnsArray = false, bool isStatic = false)
    {
        var method = new MetaMethod(name, returnClass, returnsArray, isStatic);
        Methods.Add(method);
        return method;
    }
    public MetaMethod AddConstructor()
    {
        var method = new MetaMethod("_constructor", null, false, false, true);
        Methods.Add(method);
        return method;
    }
    public MetaMethod? GetConstructorBySignature(params MetaClass[] parameterTypes)
    {
        var signature = string.Join("_constructor(", parameterTypes.Select(t => t.Name), ")");
        var constructor = GetMethods.FirstOrDefault(m => m.GetSignature() == signature);
        return constructor;
    }

    public MetaMethod? Method(string methodName)
    {
        var method = Methods.FirstOrDefault(m => m.Name == methodName);
        if(method is null)
            throw new Exception($"El método '{methodName}' no existe en el tipo '{this.Name}'.");
        return method;
    }
    #endregion
    
    #region Properties
    private List<MetaProperty> _properties { get; set; } = new();
    
    public IReadOnlyCollection<MetaProperty> Properties => _properties.AsReadOnly();

    public void AddProperty(string name, MetaClass propertyClass, bool isCollection = false)
    {
        NameFormatValidator.ValidatePascalCase(name, "Property");
        var property = Properties.FirstOrDefault(p => p.Name == name && p.MetaClass.Name == propertyClass.Name);
        
        if(property == null)
        {
            _properties.Add(new MetaProperty(Guid.NewGuid(), name, propertyClass, isCollection));
            AddDomainEvent(new Events.MetaPropertyAdded(Guid, name, propertyClass.Guid, isCollection));
        }
        else
        {
            throw new DomainException($"La propiedad con nombre '{name}' y tipo {property.MetaClass.Name} ya existe en el tipo '{this.Name}'.");
        }
    }
    
    public MetaProperty? Property(string propertyName) =>Properties.FirstOrDefault(p => p.Name == propertyName);

    /// <summary>
    /// Carga una propiedad desde la persistencia sin disparar eventos de dominio.
    /// </summary>
    public void LoadProperty(string name, MetaClass propertyClass, bool isCollection = false)
    {
        if (!_properties.Any(p => p.Name == name))
        {
            _properties.Add(new MetaProperty(Guid.NewGuid(), name, propertyClass, isCollection));
        }
    }
    #endregion
}