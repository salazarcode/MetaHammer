/*
using MetaHammer.Interpreter.Interfaces;

namespace MetaHammer.Interpreter.Configuration;

public class NativeMethodProvider : INativeMethodProvider
{
    private readonly Dictionary<string, INativeMethod> _methods;

    // .NET inyecta aquí TODAS las clases que encontramos con el Scan
    public NativeMethodProvider(IEnumerable<INativeMethod> methods)
    {
        _methods = methods.ToDictionary(f => f.Name);
    }

    public INativeMethod Get(string identifier) => _methods[identifier];

    public bool Exists(string identifier)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<INativeMethod> GetAll()
    {
        return _methods.Values.ToList();
    }

    INativeMethod INativeMethodProvider.Get(string identifier)
    {
        throw new NotImplementedException();
    }
}
*/
