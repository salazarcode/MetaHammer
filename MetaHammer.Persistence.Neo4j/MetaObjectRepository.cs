using MetaHammer.Domain.Features.Objects;
using MetaHammer.Domain.Features.Objects.Events;
using MetaHammer.Domain.Interfaces.Repositories;
using Neo4j.Driver;

namespace MetaHammer.Persistence.Neo4j;

public class MetaObjectRepository : Neo4jRepositoryBase<MetaObject>, IMetaObjectRepository
{
    public MetaObjectRepository(Neo4jContext context) : base(context)
    {
    }

    protected override async Task ApplyEventAsync(IAsyncQueryRunner tx, object @event)
    {
        switch (@event)
        {
            case MetaObjectCreated e:
                await tx.RunAsync(
                    "MATCH (c:MetaClass { Guid: $classGuid }) " +
                    "CREATE (o:MetaObject { Guid: $guid, TenantId: $tenantId })-[:INSTANCE_OF]->(c)",
                    new { guid = e.Guid.ToString(), classGuid = e.ClassGuid.ToString(), tenantId = e.TenantGuid.ToString() });
                break;

            case MetaObjectPropertySet e:
                if (e.ReferenceGuid.HasValue)
                {
                    await tx.RunAsync(
                        "MATCH (o:MetaObject { Guid: $guid }), (r:MetaObject { Guid: $refGuid }) " +
                        "CREATE (o)-[:HAS_VALUE { Property: $name }]->(r)",
                        new { guid = e.ObjectGuid.ToString(), refGuid = e.ReferenceGuid.Value.ToString(), name = e.PropertyName });
                }
                else
                {
                    await tx.RunAsync(
                        "MATCH (o:MetaObject { Guid: $guid }) " +
                        "SET o[$name] = $value",
                        new { guid = e.ObjectGuid.ToString(), name = e.PropertyName, value = e.Value });
                }
                break;
        }
    }
}
