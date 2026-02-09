using MetaHammer.Domain.Common;
using MetaHammer.Domain.Exceptions;
using MetaHammer.Domain.Features.Classes;
using MetaHammer.Domain.Features.Objects.Events;
using MetaHammer.Domain.Features.Organizations;
using MetaHammer.Domain.Features.Organizations.Identity;

namespace MetaHammer.Domain.Features.Objects;

public class MetaObject : AggregateRoot
{
    public MetaObject(MetaClass metaClass, Guid guid, Guid tenantGuid, Guid createdByGuid) : base(guid)
    {
        MetaClass = metaClass;
        TenantGuid = tenantGuid;
        CreatedByGuid = createdByGuid;
        
        AddDomainEvent(new MetaObjectCreated(guid, metaClass.Guid, tenantGuid));
    }
    
    public bool IsLoaded { get; set; } = true;
    public Guid CreatedByGuid { get; set; }
    public User? CreatedBy { get; set; }
    public Guid TenantGuid { get; set; }
    public Organization? Tenant { get; set; }
    public MetaClass MetaClass { get; private set; }
    private object? Value { get; set; } = null;

    private Dictionary<string, MetaObject> _properties { get; set; } = new();
    public IReadOnlyDictionary<string, MetaObject> Properties => _properties.AsReadOnly();
    
    public MetaObject? Property(string propertyName)
    {
        var property = MetaClass.Properties.FirstOrDefault(x => x.Name ==  propertyName);
        
        if(property is null)
            throw new DomainException($"Property '{propertyName}' does not exist in MetaClass '{MetaClass.Name}'");
        
        _properties.TryGetValue(propertyName, out var metaObject);
        
        return metaObject;
    }
    
    public void SetProperty(string propertyName, MetaObject metaObject)
    {
        var property = MetaClass.Properties.FirstOrDefault(x => x.Name == propertyName);
        
        if(property is null)
            throw new DomainException($"Property '{propertyName}' does not exist in MetaClass '{MetaClass.Name}'");
        
        if(property.MetaClass.Guid != metaObject.MetaClass.Guid)
            throw new DomainException($"Property '{propertyName}' is of type '{property.MetaClass.Name}' but got '{metaObject.MetaClass.Name}'");
        
        _properties[propertyName] = metaObject;
        
        AddDomainEvent(new MetaObjectPropertySet(Guid, propertyName, null, metaObject.Guid));
    }
    
}