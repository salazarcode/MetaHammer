using MetaHammer.Domain.Common;

namespace MetaHammer.Domain.Types;

using Abstract;
using Interfaces;

/// <summary>
/// Concrete type representing primitive data types like int, string, bool, etc.
/// It's meant to be used as template for properties, alongside with ValueObjectTypes
/// </summary>
public class PrimitiveType : Entity, IPropertyType
{

    public Type ClrType { get; }
    public string Name { get; private set; }

    public static readonly PrimitiveType Int = new("Int", typeof(int));
    public static readonly PrimitiveType String = new("String", typeof(string));
    public static readonly PrimitiveType Bool = new("Bool", typeof(bool));
    public static readonly PrimitiveType Decimal = new("Decimal", typeof(decimal));
    public static readonly PrimitiveType Double = new("Double", typeof(double));
    public static readonly PrimitiveType DateTime = new("DateTime", typeof(DateTime));
    public static readonly PrimitiveType MetaGuid = new("Guid", typeof(Guid));
    
    private PrimitiveType(string name, Type clrType) : base(System.Guid.NewGuid())
    {
        Name = name;
        ClrType = clrType;
    }
}
