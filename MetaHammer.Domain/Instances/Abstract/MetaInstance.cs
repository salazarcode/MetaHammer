using MetaHammer.Domain.Types.Abstract;

namespace MetaHammer.Domain.Instances.Abstract;

public abstract class MetaInstance
{
    public Guid Guid { get; protected set; } = Guid.NewGuid();
    public required MetaType Type { get; init; }
}
