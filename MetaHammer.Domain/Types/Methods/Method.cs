using MetaHammer.Domain.Common;

namespace MetaHammer.Domain.Types.Methods;

public class Method : Entity
{
    public Method(Guid parentTypeId, string name, MetaType? returnType, bool returnsArray = false, bool isStatic = false, bool isConstructor = false, bool isNative = false) : base(System.Guid.NewGuid())
    {
        ParentTypeId = parentTypeId;
        Name = name;
        IsStatic = isStatic;
        IsConstructor = isConstructor;
        ReturnType = returnType;
        IsArray = returnsArray;
        IsNative = isNative;
    }
    public string Name { get; private set; }
    public bool IsStatic { get; private set; }
    public bool IsConstructor { get; private set; }
    public MetaType? ReturnType { get; set; }
    public bool IsArray { get; private set; }
    public bool IsNative { get; private set; }
    public Guid ParentTypeId { get; private set; }
    private List<Parameter> _parameters { get; set; } = new();
    private List<Instruction> _instructions { get; set; } = new();
    
    [System.Text.Json.Serialization.JsonIgnore]
    public MetaType ParentType { get; private set; }

    public IReadOnlyCollection<Parameter> Parameters() => _parameters.AsReadOnly();
    public IReadOnlyCollection<Instruction> Instructions() => _instructions.AsReadOnly();
    
    /// <summary>
    /// Añade un parámetro al método actual por nombre, tipo y si es array o no.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="type"></param>
    /// <param name="isArray"></param>
    public void AddParameter(string name, MetaType type, bool isArray = false)
    {
        var parameter = new Parameter(name, type, _parameters.Count + 1, isArray);
        _parameters.Add(parameter);
    }

    /// <summary>
    /// Una instrucción es siempre una llamada a un método con sus argumentos.
    /// </summary>
    /// <param name="method"></param>
    /// <param name="arguments"></param>
    public void AddInstruction(Method invokeMethod, List<Argument> arguments)
    {
        var instruction = new Instruction(this, invokeMethod, _instructions.Count + 1 , arguments);
        _instructions.Add(instruction);
    }

    public string GetSignature()
    {
        var returnTypeName = ReturnType?.Name ?? "void";
        var staticPrefix = IsStatic ? "static" : "";
        var arrayPostfix = IsArray ? "[]" : "";

        var parameters = string.Join(", ", _parameters
            .OrderBy(p => p.Order)
            .Select(p => $"{p.Type.Name}{(p.IsArray ? "[]" : "")} {p.Name}"));

        return $"{staticPrefix}{returnTypeName}{arrayPostfix} {Name}({parameters})";
    }
}