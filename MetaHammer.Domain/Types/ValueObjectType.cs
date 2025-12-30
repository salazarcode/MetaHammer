using MetaHammer.Domain.Types.Methods;

namespace MetaHammer.Domain.Types;

using MetaHammer.Domain.Types.Abstract;
using MetaHammer.Domain.Types.Interfaces;

public class ValueObjectType : StructuralType, IPropertyType
{
    public ValueObjectType(string name)
    {
        Guid = Guid.NewGuid();
        Name = name;
    }
}