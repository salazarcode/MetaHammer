using MetaHammer.Domain.Instances.Interfaces;
using MetaHammer.Domain.Types;

namespace MetaHammer.Domain.Instances;

public class Property(PropertyDefinition propertyDefinition, IPropertyValue value)
{
    public Guid Guid { get; set; } = Guid.NewGuid();
    public PropertyDefinition PropertyDefinition { get; private set; } = propertyDefinition;
    public IPropertyValue Value { get; private set; } = value;
}