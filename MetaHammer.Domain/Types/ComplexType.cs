using MetaHammer.Domain.Instances;
using MetaHammer.Domain.Instances.Abstract;
using MetaHammer.Domain.Types.Methods;
using MetaHammer.Domain.Types.Abstract;

namespace MetaHammer.Domain.Types;

/// <summary>
/// Concrete type that represents real objects with properties, methods and relations.
/// </summary>
public class ComplexType : StructuralType
{
    public IReadOnlyCollection<RelationDefinition> Relations => relations.AsReadOnly();
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

    private List<RelationDefinition> relations = new();

    public ComplexType(string name) : base(System.Guid.NewGuid(), name)
    {
        Name = name;
    }

    public void AddRelation(string name, ComplexType type, bool isComposition = false, bool isArray = false)
    {
        relations.Add(new RelationDefinition(name, type, isComposition, isArray));
    }
    
    public Method? GetConstructorBySignature(params MetaType[] parameterTypes)
    {
        var signature = string.Join("_constructor(", parameterTypes.Select(t => t.Name), ")");
        var constructor = Methods.FirstOrDefault(m => m.GetSignature() == signature);
        return constructor;
    }
}