using MetaHammer.Domain.Types.Abstract;

namespace MetaHammer.Domain.Instances.Abstract;

/// <summary>
/// Abstract base class for all meta instances.
/// </summary>
public abstract class MetaInstance(Guid guid, MetaType metaType)
{
    public Guid Guid { get; protected set; } = guid;

    public required MetaType Type { get; init; } = metaType;
}
