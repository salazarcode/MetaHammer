using MetaHammer.Domain.Instances.Base;
using MetaHammer.Domain.Types;

namespace MetaHammer.Domain.Instances;

public class ValueObjectInstance(string name, MetaType type) : Instance(name, type)
{
}