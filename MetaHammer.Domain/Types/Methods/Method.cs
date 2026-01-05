using MetaHammer.Domain.Common;

namespace MetaHammer.Domain.Types.Methods;

public class Method : Entity
{
    public Method(string name, MetaType? returnType, bool returnsArray = false, bool isStatic = false, bool isConstructor = false, bool isNative = false) : base(System.Guid.NewGuid())
    {
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
    private List<Parameter> Parameters { get; set; } = new();
    private List<Instruction> Instructions { get; set; } = new();

    public IReadOnlyCollection<Parameter> GetParameters() => Parameters;
    public IReadOnlyCollection<Instruction> GetInstructions() => Instructions;
    
    public void AddParameter(string name, MetaType type, bool isArray = false)
    {
        var parameter = new Parameter(name, type, Parameters.Count + 1, isArray);
        Parameters.Add(parameter);
    }

    public void AddInstruction(Method methodInvoked, Dictionary<Parameter, string> arguments)
    {
        var instruction = new Instruction(methodInvoked, Instructions.Count + 1 , arguments);
        Instructions.Add(instruction);
    }

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