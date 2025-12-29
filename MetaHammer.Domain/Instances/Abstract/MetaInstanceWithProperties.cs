using MetaHammer.Domain.Exceptions;
using MetaHammer.Domain.Instances.Interfaces;
using MetaHammer.Domain.Types;
using MetaHammer.Domain.Types.Interfaces;

namespace MetaHammer.Domain.Instances.Abstract;

public abstract class MetaInstanceWithProperties : MetaInstance
{
    public List<Property> Properties { get; private set; } = new();

    public void SetProperty(PropertyDefinition property, IPropertyValue value)
    {
        if (!((ITypeWithProperties)Type).Properties.Contains(property))
        {
            throw new DomainException($"PropertyDefinition '{property.Name}' does not belong to type '{Type.Name}'");
        }
        Properties.Add(new Property(property, value));
    }
}
