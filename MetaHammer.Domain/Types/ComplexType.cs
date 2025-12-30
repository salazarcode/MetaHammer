using MetaHammer.Domain.Instances;
using MetaHammer.Domain.Instances.Abstract;
using MetaHammer.Domain.Types.Interfaces;
using MetaHammer.Domain.Types.Methods;
using MetaHammer.Domain.Types.Abstract;

namespace MetaHammer.Domain.Types;

/// <summary>
/// Representa un tipo complejo que puede tener propiedades, relaciones con otros tipos y métodos.
/// Es el equivalente a una entidad o agregado en DDD. Puede crear instancias de tipo <see cref="ComplexInstance"/>.
/// </summary>
public class ComplexType : StructuralType
{
    /// <summary>
    /// Mapeo de tipos CLR a sus correspondientes setters de propiedades.
    /// Utiliza los ClrType definidos en <see cref="PrimitiveType"/> para mayor consistencia.
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

    private List<RelationDefinition> relations = new();

    /// <summary>
    /// Crea un nuevo tipo complejo con el nombre especificado.
    /// </summary>
    /// <param name="name">Nombre del tipo en formato PascalCase.</param>
    public ComplexType(string name)
    {
        Guid = Guid.NewGuid();
        Name = name;
    }

    /// <summary>
    /// Colección de definiciones de relaciones con otros tipos complejos.
    /// </summary>
    public IReadOnlyCollection<RelationDefinition> Relations => relations.AsReadOnly();

    /// <summary>
    /// Agrega una definición de relación con otro tipo complejo.
    /// </summary>
    /// <param name="name">Nombre de la relación en formato snake_case.</param>
    /// <param name="isArray">Indica si la relación puede contener múltiples referencias.</param>
    /// <param name="type">Tipo complejo destino de la relación.</param>
    /// <param name="isComposition">Indica si es una composición (el hijo no existe sin el padre).</param>
    public void AddRelation(string name, bool isArray, ComplexType type, bool isComposition = false)
    {
        relations.Add(new RelationDefinition
        {
            Guid = Guid.NewGuid(),
            Name = name,
            IsArray = isArray,
            IsComposition = isComposition,
            Type = type
        });
    }

    /// <summary>
    /// Crea una nueva instancia de este tipo con un GUID generado automáticamente.
    /// </summary>
    /// <param name="properties">Objeto anónimo con las propiedades a asignar (opcional).</param>
    /// <returns>Nueva instancia del tipo complejo.</returns>
    public ComplexInstance CreateInstance(object? properties = null)
    {
        var instance = new ComplexInstance(Guid.NewGuid(), this);
        if (properties != null)
            SetPropertiesFromObject(instance, properties);
        return instance;
    }

    /// <summary>
    /// Crea una nueva instancia de este tipo con el GUID especificado.
    /// </summary>
    /// <param name="guid">Identificador único para la instancia.</param>
    /// <param name="properties">Objeto anónimo con las propiedades a asignar (opcional).</param>
    /// <returns>Nueva instancia del tipo complejo.</returns>
    public ComplexInstance CreateInstance(Guid guid, object? properties = null)
    {
        var instance = new ComplexInstance(guid, this);
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