using MetaHammer.Domain.Exceptions;
using MetaHammer.Domain.Instances.Abstract;

namespace MetaHammer.Domain.Types.Methods;

public class MethodContext
{
    private readonly Dictionary<string, object?> _data = new();
    private readonly Dictionary<string, MetaInstance> _instances = new();

    public void AddInstance(string key, MetaInstance instance)
    {
        if (_instances.ContainsKey(key))
        {
            throw new DomainException($"Instance with key '{key}' already exists in context");
        }
        _instances[key] = instance;
    }

    public MetaInstance GetInstance(string key)
    {
        if (!_instances.TryGetValue(key, out var instance))
        {
            throw new DomainException($"Instance with key '{key}' not found in context");
        }
        return instance;
    }

    public bool TryGetInstance(string key, out MetaInstance? instance)
    {
        return _instances.TryGetValue(key, out instance);
    }
}