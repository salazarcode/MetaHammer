namespace MetaHammer.Domain.Common;

public abstract class Entity
{
    public Guid Guid { get; protected set; }

    public Entity(Guid guid)
    {
        Guid = guid;
    }
}