using MetaHammer.Domain.Instances.Abstract;
using MetaHammer.Domain.Types.Methods;

namespace MetaHammer.Interpreter;

/// <summary>
/// La clase Context representa el entorno de ejecución actual,
/// Aquí se almacenan las variables y métodos nativos disponibles.
/// </summary>
public class Context
{
    private readonly Dictionary<string, MetaInstance> _variables = new();
    private readonly Dictionary<string, Method> _nativeMethods = new();
    private readonly Context? _parentContext;

    public Context(Context? parentContext = null)
    {
        _parentContext = parentContext;
    }

    public void Set(string name, MetaInstance value)
    {
        //Verifica que la variable no exista en el contexto actual y recursivamente en el padre
        if (_variables.ContainsKey(name))
        {
            throw new ApplicationException($"Variable '{name}' ya definida en el contexto actual.");
        }
        _variables[name] = value;
    }

    public MetaInstance Get(string name)
    {
        if (_variables.TryGetValue(name, out var value)) 
            return value;
        
        // Si no está aquí, buscar en el padre (Recursividad de Scope)
        if (_parentContext != null) 
            return _parentContext.Get(name);
        
        throw new ApplicationException($"Variable '{name}' no definida en el contexto actual.");
    }
}