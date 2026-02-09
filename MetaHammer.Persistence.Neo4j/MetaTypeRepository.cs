using MetaHammer.Domain.Features.Classes;
using MetaHammer.Domain.Features.Classes.Enums;
using MetaHammer.Domain.Features.Classes.Events;
using MetaHammer.Domain.Features.Organizations;
using MetaHammer.Domain.Features.Organizations.Identity;
using MetaHammer.Domain.Interfaces.Repositories;
using MetaHammer.Domain.ReadModels;
using Neo4j.Driver;

namespace MetaHammer.Persistence.Neo4j;

public class MetaTypeRepository : Neo4jRepositoryBase<MetaClass>, IMetaTypeRepository
{
    public MetaTypeRepository(Neo4jContext context) : base(context)
    {
    }

    public async Task<MetaClass?> GetByIdAsync(Guid guid)
    {
        await using var session = Context.CreateSession();
        return await session.ExecuteReadAsync(async tx =>
        {
            var result = await tx.RunAsync(
                "MATCH (n:MetaClass { Guid: $guid }) RETURN n.Name as Name, n.Nature as Nature, n.OrganizationId as OrgId, n.IsNative as IsNative",
                new { guid = guid.ToString() });
            
            if (await result.FetchAsync())
            {
                var name = result.Current["Name"].As<string>();
                var nature = Enum.Parse<MetaNature>(result.Current["Nature"].As<string>());
                var orgId = Guid.Parse(result.Current["OrgId"].As<string>());
                
                bool isNative = false;
                if (result.Current.Values.ContainsKey("IsNative") && result.Current["IsNative"] != null)
                {
                    isNative = result.Current["IsNative"].As<bool>();
                }
                
                var org = new Organization(orgId, "Loaded Org");
                var user = new User(org, Guid.Empty, "system");
                
                var metaClass = new MetaClass(guid, name, nature, org, user, isNative);

                // Load properties
                var propsResult = await tx.RunAsync(
                    "MATCH (c:MetaClass { Guid: $guid })-[r:HAS_PROPERTY]->(t:MetaClass) " +
                    "RETURN r.Name as Name, r.IsCollection as IsCollection, t.Guid as TypeGuid, t.Name as TypeName",
                    new { guid = guid.ToString() });

                while (await propsResult.FetchAsync())
                {
                    var pName = propsResult.Current["Name"].As<string>();
                    var pIsCol = propsResult.Current["IsCollection"].As<bool>();
                    var pTypeGuid = Guid.Parse(propsResult.Current["TypeGuid"].As<string>());
                    var pTypeName = propsResult.Current["TypeName"].As<string>();

                    var pType = new MetaClass(pTypeGuid, pTypeName, MetaNature.Primitive, org, user);
                    metaClass.LoadProperty(pName, pType, pIsCol);
                }

                return metaClass;
            }
            return null;
        });
    }

    public async Task<List<MetaTypeReadModel>> GetAllAsync()
    {
        await using var session = Context.CreateSession();
        return await session.ExecuteReadAsync(async tx =>
        {
            var result = await tx.RunAsync(
                "MATCH (n:MetaClass) " +
                "OPTIONAL MATCH (n)-[r:HAS_PROPERTY]->(t:MetaClass) " +
                "WITH n, r, t ORDER BY r.Name " +
                "RETURN n.Guid as Guid, n.Name as Name, n.Nature as Nature, n.ParentGuid as ParentGuid, n.IsNative as IsNative, " +
                "collect(DISTINCT { Name: r.Name, IsCollection: r.IsCollection, TypeGuid: t.Guid, TypeName: t.Name }) as Properties");
            
            var list = new List<MetaTypeReadModel>();
            
            while (await result.FetchAsync())
            {
                var guid = Guid.Parse(result.Current["Guid"].As<string>());
                var name = result.Current["Name"].As<string>();
                var nature = result.Current["Nature"].As<string>();
                
                bool isNative = false;
                if (result.Current.Values.ContainsKey("IsNative") && result.Current["IsNative"] != null)
                {
                    isNative = result.Current["IsNative"].As<bool>();
                }
                
                string? parentGuidStr = null;
                if (result.Current.Values.ContainsKey("ParentGuid") && result.Current["ParentGuid"] != null)
                {
                    parentGuidStr = result.Current["ParentGuid"].As<string>();
                }
                Guid? parentGuid = parentGuidStr != null ? Guid.Parse(parentGuidStr) : null;

                var properties = new List<MetaPropertyReadModel>();
                var propsList = result.Current["Properties"].As<List<object>>();
                foreach (var pObj in propsList)
                {
                    var pDict = pObj as IDictionary<string, object>;
                    if (pDict != null && pDict.ContainsKey("Name") && pDict["Name"] != null)
                    {
                        properties.Add(new MetaPropertyReadModel(
                            pDict["Name"].ToString()!,
                            Guid.Parse(pDict["TypeGuid"].ToString()!),
                            pDict["TypeName"].ToString()!,
                            (bool)pDict["IsCollection"]
                        ));
                    }
                }
                
                list.Add(new MetaTypeReadModel(guid, name, nature, isNative, parentGuid, properties));
            }
            return list;
        });
    }

    protected override async Task ApplyEventAsync(IAsyncQueryRunner tx, object @event)
    {
        switch (@event)
        {
            case MetaClassCreated e:
                await tx.RunAsync(
                    "MERGE (n:MetaClass { Guid: $guid }) SET n.Name = $name, n.Nature = $nature, n.OrganizationId = $orgId, n.IsNative = $isNative",
                    new { guid = e.Guid.ToString(), name = e.Name, nature = e.Nature.ToString(), orgId = e.OrganizationId.ToString(), isNative = e.IsNative });
                break;

            case MetaPropertyAdded e:
                await tx.RunAsync(
                    "MATCH (c:MetaClass { Guid: $classGuid }), (t:MetaClass { Guid: $typeGuid }) " +
                    "MERGE (c)-[r:HAS_PROPERTY { Name: $name }]->(t) " +
                    "SET r.IsCollection = $isCol",
                    new { classGuid = e.ClassGuid.ToString(), typeGuid = e.PropertyTypeGuid.ToString(), name = e.PropertyName, isCol = e.IsCollection });
                break;
        }
    }
}
