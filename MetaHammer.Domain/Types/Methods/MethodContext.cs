using MetaHammer.Domain.Exceptions;
using MetaHammer.Domain.Instances.Abstract;

namespace MetaHammer.Domain.Types.Methods;

/// <summary>
/// Contexto de ejecución de un método.
/// Almacena instancias y datos durante la ejecución de las instrucciones.
/// </summary>
public class MethodContext
{
    /// <summary>
    /// Diccionario de datos genéricos del contexto.
    /// </summary>
    private readonly Dictionary<string, object?> _data = new();

    /// <summary>
    /// Diccionario de instancias almacenadas por clave.
    /// </summary>
    private readonly Dictionary<string, MetaInstance> _instances = new();

    /// <summary>
    /// Agrega una instancia al contexto con una clave única.
    /// </summary>
    /// <param name="key">Clave única para identificar la instancia.</param>
    /// <param name="instance">Instancia a almacenar.</param>
    /// <exception cref="DomainException">Si ya existe una instancia con esa clave.</exception>
    public void AddInstance(string key, MetaInstance instance)
    {
        if (_instances.ContainsKey(key))
        {
            throw new DomainException($"Instance with key '{key}' already exists in context");
        }
        _instances[key] = instance;
    }

    /// <summary>
    /// Obtiene una instancia del contexto por su clave.
    /// </summary>
    /// <param name="key">Clave de la instancia.</param>
    /// <returns>La instancia almacenada.</returns>
    /// <exception cref="DomainException">Si no existe una instancia con esa clave.</exception>
    public MetaInstance GetInstance(string key)
    {
        if (!_instances.TryGetValue(key, out var instance))
        {
            throw new DomainException($"Instance with key '{key}' not found in context");
        }
        return instance;
    }

    /// <summary>
    /// Intenta obtener una instancia del contexto sin lanzar excepción.
    /// </summary>
    /// <param name="key">Clave de la instancia.</param>
    /// <param name="instance">Instancia encontrada o null.</param>
    /// <returns>True si se encontró la instancia, false en caso contrario.</returns>
    public bool TryGetInstance(string key, out MetaInstance? instance)
    {
        return _instances.TryGetValue(key, out instance);
    }
}