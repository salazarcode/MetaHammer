namespace MetaHammer.Application.DTOs;

public record MetaPropertyDto(string Name, Guid TypeGuid, string TypeName, bool IsCollection);
public record MetaTypeDto(Guid Guid, string Name, string Nature, bool IsNative, Guid? ParentGuid, List<MetaPropertyDto> Properties);
public record MetaObjectDto(Guid Guid, Guid ClassGuid, Dictionary<string, object> Properties);

public record CreateMetaTypeRequest(string Name, string Nature, Guid OrganizationId, Guid UserId, bool IsNative = false, Guid? ParentGuid = null);
public record AddPropertyRequest(string Name, Guid TypeGuid, bool IsCollection);
public record AddMetaTypePropertiesRequest(List<AddPropertyRequest> Properties);
public record CreateMetaObjectRequest(Guid ClassGuid, Guid TenantId, Guid UserId, Dictionary<string, object> InitialProperties);

public record ApiErrorResponse(string Title, string Detail, int Status);
