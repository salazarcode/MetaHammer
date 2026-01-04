using MetaHammer.Domain.Instances.Abstract;

namespace MetaHammer.Interpreter.Interfaces;

/// <summary>
/// Define el motor de ejecución encargado de procesar la lógica de los métodos modelados.
/// </summary>
public interface IInterpreter
{
    /// <summary>
    /// Ejecuta un método específico dentro de un contexto dado.
    /// </summary>
    /// <param name="context">El contexto de ejecución que contiene las variables y servicios.</param>
    /// <param name="guid">El identificador de la instancia dueña del método.</param>
    /// <param name="methodName">El nombre del método invocado.</param>
    /// <param name="arguments">Los argumentos concretos (instancias) pasados al método.</param>
    /// <returns>Una instancia que representa el resultado de la ejecución.</returns>
    /// <exception cref="MetaExecutionException">Se lanza si ocurre un error durante la interpretación.</exception>
    MetaInstance? Execute(
        Context context, 
        Guid guid,
        string methodName, 
        params MetaInstance[] arguments);
}