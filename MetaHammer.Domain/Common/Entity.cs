namespace MetaHammer.Domain.Common;

public abstract class Entity(Guid guid)
{
    public Guid Guid { get; protected set; } = guid;
}