using System.Dynamic;
using MetaHammer.Domain.Features.Objects;

namespace MetaHammer.Domain.Services.Interpreter.ExpressionEvaluator;

public class MetaObjectAccessor : DynamicObject
{
    private readonly MetaObject _target;

    public MetaObjectAccessor(MetaObject target)
    {
        _target = target;
    }

    // Esta es la magia: Intercepta "objeto.Propiedad"
    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        if (_target.Properties.TryGetValue(binder.Name, out var value))
        {
            result = WrapValue(value);
            return true;
        }

        // Si la propiedad no existe, devolvemos null (o puedes lanzar excepción si prefieres strict mode)
        result = null;
        return true; 
    }

    // Helper recursivo vital para navegación profunda (ej: user.Direccion.Ciudad)
    // y para que LINQ funcione sobre listas (ej: user.Pedidos.Sum(...))
    private object? WrapValue(object? value)
    {
        if (value == null) return null;

        // 1. Si es un hijo MetaObject, lo envolvemos también
        if (value is MetaObject metaObj)
            return new MetaObjectAccessor(metaObj);

        // 2. Si es una lista de MetaObjects, proyectamos cada uno a un Accessor
        // Esto permite hacer .Where(), .Sum(), .First() dentro de la expresión string
        if (value is IEnumerable<MetaObject> list)
            return list.Select(x => new MetaObjectAccessor(x)).ToList();

        // 3. Si es primitivo (int, string, bool), se devuelve crudo
        return value;
    }
}