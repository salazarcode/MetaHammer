using MetaHammer.Domain.Exceptions;

namespace MetaHammer.Domain.Types.Entities.Method;

public class DesignContext
{
    public DesignContext(DesignContext? parentScope)
    {
        
    }
    
    // Diccionario de: Nombre de Variable -> Definición del Tipo
    private readonly Dictionary<string, ComplexType> _symbols = new();

    // Registra una variable (Parámetros, This, o Variables Locales creadas por instrucciones)
    public void Define(string name, ComplexType type)
    {
        if (_symbols.ContainsKey(name))
            throw new DomainException($"La variable '{name}' ya está definida en este scope.");
        
        _symbols[name] = type;
    }

    // Busca una variable para validar su uso
    public ComplexType Resolve(string name)
    {
        if (!_symbols.TryGetValue(name, out var type))
            throw new DomainException($"Error de Diseño: La variable '{name}' no existe en el contexto actual.");
        
        return type;
    }

    public bool IsDefined(string name) => _symbols.ContainsKey(name);
}