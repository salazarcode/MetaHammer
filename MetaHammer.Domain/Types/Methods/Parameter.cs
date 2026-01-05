using MetaHammer.Domain.Common;

namespace MetaHammer.Domain.Types.Methods;

public class Parameter(string name, MetaType type, int order, bool isArray = false) : Entity(System.Guid.NewGuid())
{
    public string Name { get; private set; } = name;

    public MetaType Type { get; private set; } = type;

    public bool IsArray { get; private set; } = isArray;

    public int Order { get; private set; } = order;
}