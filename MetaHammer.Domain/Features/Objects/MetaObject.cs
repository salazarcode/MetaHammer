using MetaHammer.Domain.Features.Classes;
using MetaHammer.Domain.Features.Organizations;
using MetaHammer.Domain.Features.Organizations.Identity;

namespace MetaHammer.Domain.Features.Objects;

public class MetaObject
{
    public MetaObject(MetaClass metaClass, Guid guid, Guid tenantGuid, Guid createdByGuid)
    {
        Guid = guid;
        MetaClass = metaClass;
        TenantGuid = tenantGuid;
        CreatedByGuid = createdByGuid;
    }
    
    public Guid? Guid { get; init; } = null;
    public bool IsLoaded { get; set; } = true;
    public Guid CreatedByGuid { get; set; }
    public User? CreatedBy { get; set; }
    public Guid TenantGuid { get; set; }
    public Organization? Tenant { get; set; }
    public MetaClass MetaClass { get; private set; }
    private object? Value { get; set; } = null;
}