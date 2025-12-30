using MetaHammer.Domain.Exceptions;

namespace MetaHammer.Domain.Types.Methods;

/// <summary>
/// Representa una instrucción dentro del cuerpo de un método.
/// Cada instrucción es una llamada a otro método con argumentos específicos.
/// </summary>
public class MethodInstruction
{
    /// <summary>
    /// Identificador único de la instrucción.
    /// </summary>
    public Guid Guid { get; set; }

    /// <summary>
    /// Método que se invoca en esta instrucción.
    /// </summary>
    public Method Method { get; set; }

    /// <summary>
    /// Orden de ejecución de la instrucción dentro del método.
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Mapeo de parámetros a sus valores (referencias a variables del contexto).
    /// </summary>
    private Dictionary<MethodParameter, string> Arguments { get; set; }

    /// <summary>
    /// Crea una nueva instrucción que invoca un método con los argumentos dados.
    /// </summary>
    /// <param name="method">Método a invocar.</param>
    /// <param name="arguments">Diccionario de parámetro a nombre de variable en el contexto.</param>
    /// <exception cref="DomainException">Si falta algún parámetro requerido.</exception>
    public MethodInstruction(Method method, Dictionary<MethodParameter, string> arguments)
    {
        Guid = Guid.NewGuid();
        Method = method;

        foreach (var param in method.Parameters)
        {
            if (!arguments.ContainsKey(param))
                throw new DomainException($"Parameter {param} does not exist in method {method.Name}.");
        }

        Arguments = arguments;
    }
}