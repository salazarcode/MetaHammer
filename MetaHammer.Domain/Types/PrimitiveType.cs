namespace MetaHammer.Domain.Types;

using Abstract;
using Interfaces;

public class PrimitiveType : MetaType, IPropertyType
{
    private PrimitiveType(string name, Type clrType)
    {
        Name = name;
        ClrType = clrType;
    }

    public Type ClrType { get; }

    public static readonly PrimitiveType Int = new("Int", typeof(int));
    public static readonly PrimitiveType String = new("String", typeof(string));
    public static readonly PrimitiveType Bool = new("Bool", typeof(bool));
    public static readonly PrimitiveType Decimal = new("Decimal", typeof(decimal));
    public static readonly PrimitiveType Double = new("Double", typeof(double));
    public static readonly PrimitiveType DateTime = new("DateTime", typeof(DateTime));
    public static readonly PrimitiveType Guid = new("Guid", typeof(Guid));
}