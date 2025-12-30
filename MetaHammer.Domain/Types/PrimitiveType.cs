namespace MetaHammer.Domain.Types;

using Abstract;
using Interfaces;

/// <summary>
/// Representa un tipo primitivo del sistema (int, string, bool, etc.).
/// Utiliza el patrón Singleton para garantizar una única instancia por tipo.
/// Implementa <see cref="IPropertyType"/> para poder usarse como tipo de propiedad.
/// </summary>
public class PrimitiveType : MetaType, IPropertyType
{
    /// <summary>
    /// Constructor privado para garantizar el patrón Singleton.
    /// </summary>
    private PrimitiveType(string name, Type clrType)
    {
        Name = name;
        ClrType = clrType;
    }

    /// <summary>
    /// Tipo CLR correspondiente a este tipo primitivo.
    /// Usado para mapear valores en tiempo de ejecución.
    /// </summary>
    public Type ClrType { get; }

    /// <summary>Tipo entero (System.Int32).</summary>
    public static readonly PrimitiveType Int = new("Int", typeof(int));

    /// <summary>Tipo cadena de texto (System.String).</summary>
    public static readonly PrimitiveType String = new("String", typeof(string));

    /// <summary>Tipo booleano (System.Boolean).</summary>
    public static readonly PrimitiveType Bool = new("Bool", typeof(bool));

    /// <summary>Tipo decimal de alta precisión (System.Decimal).</summary>
    public static readonly PrimitiveType Decimal = new("Decimal", typeof(decimal));

    /// <summary>Tipo punto flotante de doble precisión (System.Double).</summary>
    public static readonly PrimitiveType Double = new("Double", typeof(double));

    /// <summary>Tipo fecha y hora (System.DateTime).</summary>
    public static readonly PrimitiveType DateTime = new("DateTime", typeof(DateTime));

    /// <summary>Tipo identificador único global (System.Guid).</summary>
    public static readonly PrimitiveType Guid = new("Guid", typeof(Guid));
}