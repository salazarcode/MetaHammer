using MetaHammer.Domain.Types.Abstract;

namespace MetaHammer.Domain.Types.Methods;

public class Method
{
    public Guid Guid { get; private set; }
    public string Name { get; private set; }
    public bool IsStatic { get; private set; } = false;
    public bool IsConstructor { get; private set; } = false;
    public MetaType? ReturnType { get; private set; } = null;
    public bool IsArray { get; private set; }
    public string? ImplementationMethodName { get; set; } = null;
    public List<MethodParameter> Parameters { get; set; } = new();

    public List<MethodInstruction> Instructions { get; set; }
    private MethodContext Context { get; set; } = new MethodContext();

    public Method(string name, MetaType returnType, bool returnsArray = false, bool IsStatic = false)
    {
        Guid = Guid.NewGuid();
        Name = name;
        ReturnType = returnType;
        IsArray = returnsArray;
        this.IsStatic = IsStatic;
        Instructions = new List<MethodInstruction>();
    }
    
    public Method()
    {
        Guid = Guid.NewGuid();
        Name = "_constructor";
        IsConstructor = true;
        ReturnType = null;
        Instructions = new List<MethodInstruction>();
    }
    
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
    
    public void AddInstruction(MethodInstruction instruction)
    {
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