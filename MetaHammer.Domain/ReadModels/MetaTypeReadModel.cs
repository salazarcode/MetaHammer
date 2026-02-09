namespace MetaHammer.Domain.ReadModels;

public record MetaPropertyReadModel(string Name, Guid TypeGuid, string TypeName, bool IsCollection);

public record MetaTypeReadModel(Guid Guid, string Name, string Nature, bool IsNative, Guid? ParentGuid, List<MetaPropertyReadModel> Properties);
