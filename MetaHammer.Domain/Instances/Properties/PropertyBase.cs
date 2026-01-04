using MetaHammer.Domain.Instances.Interfaces;

namespace MetaHammer.Domain.Instances.Properties;

public abstract class PropertyBase : IProperty
{
    public Guid Guid { get; } = Guid.NewGuid();

    public string Name { get; }

    protected PropertyBase(string name)
    {
        Name = name;
    }
}
