using MetaHammer.Domain.Instances;
using MetaHammer.Domain.Instances.Abstract;
using MetaHammer.Domain.Types.Abstract;
using MetaHammer.Domain.Types.Interfaces;
using MetaHammer.Domain.Types.Methods;

namespace MetaHammer.Domain.Types;

/// <summary>
/// Representa un tipo de objeto de valor (Value Object) en el dominio.
/// Los Value Objects son inmutables, se comparan por valor y no tienen identidad propia.
/// Solo pueden existir como propiedades de otros tipos. Implementa <see cref="IPropertyType"/>.
/// </summary>
public class ValueObjectType : StructuralType, IPropertyType
{
    /// <summary>
    /// Mapeo de tipos CLR a sus correspondientes setters de propiedades.
    /// </summary>
    private static readonly Dictionary<Type, Action<MetaInstanceWithProperties, string, object>> PropertySetters = new()
    {
        { PrimitiveType.Int.ClrType, (i, k, v) => i.SetPropertyValue(k, (int)v) },
        { PrimitiveType.String.ClrType, (i, k, v) => i.SetPropertyValue(k, (string)v) },
        { PrimitiveType.Bool.ClrType, (i, k, v) => i.SetPropertyValue(k, (bool)v) },
        { PrimitiveType.Decimal.ClrType, (i, k, v) => i.SetPropertyValue(k, (decimal)v) },
        { PrimitiveType.Double.ClrType, (i, k, v) => i.SetPropertyValue(k, (double)v) },
        { PrimitiveType.DateTime.ClrType, (i, k, v) => i.SetPropertyValue(k, (DateTime)v) },
        { PrimitiveType.Guid.ClrType, (i, k, v) => i.SetPropertyValue(k, (Guid)v) },
        { typeof(ValueObjectInstance), (i, k, v) => i.SetPropertyValue(k, (ValueObjectInstance)v) }
    };

    /// <summary>
    /// Crea un nuevo tipo de objeto de valor con el nombre especificado.
    /// </summary>
    /// <param name="name">Nombre del tipo en formato PascalCase.</param>
    public ValueObjectType(string name)
    {
        Guid = Guid.NewGuid();
        Name = name;
    }

    /// <summary>
    /// Crea una nueva instancia de este tipo de objeto de valor.
    /// </summary>
    /// <param name="properties">Objeto anónimo con las propiedades a asignar (opcional).</param>
    /// <returns>Nueva instancia del objeto de valor.</returns>
    public ValueObjectInstance CreateInstance(object? properties = null)
    {
        var instance = new ValueObjectInstance(this);
        if (properties != null)
            SetPropertiesFromObject(instance, properties);
        return instance;
    }

    /// <summary>
    /// Asigna propiedades a una instancia desde un objeto anónimo usando reflexión.
    /// </summary>
    private static void SetPropertiesFromObject(MetaInstanceWithProperties instance, object properties)
    {
        foreach (var prop in properties.GetType().GetProperties())
        {
            var key = prop.Name;
            var value = prop.GetValue(properties);

            if (value == null)
                continue;

            var valueType = value.GetType();
            if (PropertySetters.TryGetValue(valueType, out var setter))
            {
                setter(instance, key, value);
            }
            else
            {
                throw new Exceptions.DomainException($"Unsupported property type for '{key}': {valueType.Name}");
            }
        }
    }
}