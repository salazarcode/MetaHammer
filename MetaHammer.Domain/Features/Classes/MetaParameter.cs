using MetaHammer.Domain.Common;

namespace MetaHammer.Domain.Features.Classes;

public class MetaParameter(string name, MetaClass metaClass, int order, bool IsCollection = false) : Entity(System.Guid.NewGuid())
{
    public string Name { get; private set; } = name;

    public MetaClass Type { get; private set; } = metaClass;

    public bool IsCollection { get; private set; } = IsCollection;

    public int Order { get; private set; } = order;
}