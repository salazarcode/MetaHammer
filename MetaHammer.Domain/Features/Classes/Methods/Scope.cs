using MetaHammer.Domain.Common;
using MetaHammer.Domain.Exceptions;

namespace MetaHammer.Domain.Features.Classes.Methods;

public class Scope(Guid guid) : Entity(guid)
{
    // Diccionario de: Nombre de Variable -> Definición del Tipo
    private readonly Dictionary<string, MetaClass> _symbols = new();

    // Registra una variable (Parámetros, This, o Variables Locales creadas por instrucciones)
    public void Define(string name, MetaClass type)
    {
        if (_symbols.ContainsKey(name))
            throw new DomainException($"La variable '{name}' ya está definida en este scope.");
        
        _symbols[name] = type;
    }

    // Busca una variable para validar su uso y si no la tiene la busca en su parent
    public MetaClass Resolve(string name)
    {
        if (!_symbols.TryGetValue(name, out var type))
            throw new DomainException($"Error de Diseño: La variable '{name}' no existe en el contexto actual.");
        
        return type;
    }

    public bool IsDefined(string name) => _symbols.ContainsKey(name);
}