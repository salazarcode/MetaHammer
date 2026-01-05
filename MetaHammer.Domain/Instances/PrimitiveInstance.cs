using MetaHammer.Domain.Instances.Base;
using MetaHammer.Domain.Instances.Interfaces;
using MetaHammer.Domain.Types;

namespace MetaHammer.Domain.Instances;

public class PrimitiveInstance(string propertyName, MetaType type) : Instance(propertyName, type), IPropertyInstance
{
    
}
