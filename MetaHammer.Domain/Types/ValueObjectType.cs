using MetaHammer.Domain.Instances;
using MetaHammer.Domain.Instances.Abstract;
using MetaHammer.Domain.Types.Abstract;
using MetaHammer.Domain.Types.Interfaces;

namespace MetaHammer.Domain.Types;

/// <summary>
/// Concrete type representing a value object with structured properties.
/// Its instances are meant to be used as property within other types.
/// </summary>
public class ValueObjectType : StructuralType, IPropertyType
{
    private static readonly Dictionary<Type, Action<StructuralInstance, string, object>> PropertySetters = new()
    {
        { PrimitiveType.Int.ClrType, (i, k, v) => i.SetPropertyValue(k, (int)v) },
        { PrimitiveType.String.ClrType, (i, k, v) => i.SetPropertyValue(k, (string)v) },
        { PrimitiveType.Bool.ClrType, (i, k, v) => i.SetPropertyValue(k, (bool)v) },
        { PrimitiveType.Decimal.ClrType, (i, k, v) => i.SetPropertyValue(k, (decimal)v) },
        { PrimitiveType.Double.ClrType, (i, k, v) => i.SetPropertyValue(k, (double)v) },
        { PrimitiveType.DateTime.ClrType, (i, k, v) => i.SetPropertyValue(k, (DateTime)v) },
        { PrimitiveType.MetaGuid.ClrType, (i, k, v) => i.SetPropertyValue(k, (Guid)v) },
        { typeof(ValueObjectInstance), (i, k, v) => i.SetPropertyValue(k, (ValueObjectInstance)v) }
    };

    public ValueObjectType(string name) : base(Guid.NewGuid(), name)
    {
        Name = name;
    }
}