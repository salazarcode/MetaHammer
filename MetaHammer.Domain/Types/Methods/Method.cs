using MetaHammer.Domain.Types.Abstract;

namespace MetaHammer.Domain.Types.Methods;

/// <summary>
/// Representa un método definido en un tipo estructural.
/// Puede ser un método regular o un constructor (IsConstructor = true).
/// </summary>
public class Method
{
    /// <summary>
    /// Identificador único del método.
    /// </summary>
    public Guid Guid { get; private set; }

    /// <summary>
    /// Nombre del método. Para constructores es "_constructor".
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Indica si el método es estático (no requiere instancia).
    /// </summary>
    public bool IsStatic { get; private set; } = false;

    /// <summary>
    /// Indica si el método es un constructor del tipo.
    /// </summary>
    public bool IsConstructor { get; private set; } = false;

    /// <summary>
    /// Tipo de retorno del método. Null para constructores y métodos void.
    /// </summary>
    public MetaType? ReturnType { get; private set; } = null;

    /// <summary>
    /// Indica si el método retorna una colección del tipo de retorno.
    /// </summary>
    public bool IsArray { get; private set; }

    /// <summary>
    /// Nombre del método de implementación en código externo (opcional).
    /// </summary>
    public string? ImplementationMethodName { get; set; } = null;

    /// <summary>
    /// Lista de parámetros que acepta el método.
    /// </summary>
    public List<MethodParameter> Parameters { get; set; } = new();

    /// <summary>
    /// Lista de instrucciones que conforman el cuerpo del método.
    /// </summary>
    public List<MethodInstruction> Instructions { get; set; }

    /// <summary>
    /// Contexto de ejecución para almacenar instancias durante la ejecución.
    /// </summary>
    private MethodContext Context { get; set; } = new MethodContext();

    /// <summary>
    /// Crea un método regular con nombre, tipo de retorno y opciones.
    /// </summary>
    /// <param name="name">Nombre del método.</param>
    /// <param name="returnType">Tipo que retorna el método.</param>
    /// <param name="returnsArray">Indica si retorna una colección.</param>
    /// <param name="IsStatic">Indica si es un método estático.</param>
    public Method(string name, MetaType returnType, bool returnsArray = false, bool IsStatic = false)
    {
        Guid = Guid.NewGuid();
        Name = name;
        ReturnType = returnType;
        IsArray = returnsArray;
        this.IsStatic = IsStatic;
        Instructions = new List<MethodInstruction>();
    }

    /// <summary>
    /// Crea un constructor (método especial para crear instancias del tipo).
    /// </summary>
    public Method()
    {
        Guid = Guid.NewGuid();
        Name = "_constructor";
        IsConstructor = true;
        ReturnType = null;
        Instructions = new List<MethodInstruction>();
    }

    /// <summary>
    /// Agrega un parámetro al método.
    /// </summary>
    /// <param name="name">Nombre del parámetro.</param>
    /// <param name="type">Tipo del parámetro.</param>
    /// <param name="isArray">Indica si el parámetro es una colección.</param>
    public void AddParameter(string name, MetaType type, bool isArray = false)
    {
        var parameter = new MethodParameter
        {
            Guid = Guid.NewGuid(),
            Name = name,
            Type = type,
            IsArray = isArray,
            Order = (Parameters.Count + 1)
        };
        Parameters.Add(parameter);
    }

    /// <summary>
    /// Agrega una instrucción al cuerpo del método.
    /// </summary>
    /// <param name="instruction">Instrucción a agregar.</param>
    public void AddInstruction(MethodInstruction instruction)
    {
        Instructions.Add(instruction);
    }

    /// <summary>
    /// Genera la firma del método en formato legible.
    /// Ejemplo: "static PersonType[] GetAll()" o "void _constructor(String name, Int age)".
    /// </summary>
    /// <returns>Cadena con la firma del método.</returns>
    public string GetSignature()
    {
        var returnTypeName = ReturnType?.Name ?? "void";
        var staticPrefix = IsStatic ? "static " : "";
        var arrayPostfix = IsArray ? "[]" : "";

        var parameters = string.Join(", ", Parameters
            .OrderBy(p => p.Order)
            .Select(p => $"{p.Type.Name}{(p.IsArray ? "[]" : "")} {p.Name}"));

        return $"{staticPrefix}{returnTypeName}{arrayPostfix} {Name}({parameters})";
    }
}