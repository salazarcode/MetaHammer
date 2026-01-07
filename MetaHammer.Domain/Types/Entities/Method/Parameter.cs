using MetaHammer.Domain.Common;
using MetaHammer.Domain.Types.Interfaces;

namespace MetaHammer.Domain.Types.Entities.Method;

public class Parameter(string name, IMetaType type, int order, bool isArray = false) : Entity(System.Guid.NewGuid())
{
    public string Name { get; private set; } = name;

    public IMetaType Type { get; private set; } = type;

    public bool IsArray { get; private set; } = isArray;

    public int Order { get; private set; } = order;
}