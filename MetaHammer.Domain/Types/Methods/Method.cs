using MetaHammer.Domain.Common;
using MetaHammer.Domain.Exceptions;
using MetaHammer.Domain.Instances.Abstract;
using MetaHammer.Domain.Types.Abstract;

namespace MetaHammer.Domain.Types.Methods;

public class Method(string name, MetaType? returnType, bool returnsArray = false, bool isStatic = false, bool isConstructor = false, bool isNative = false) : Entity(System.Guid.NewGuid())
{
    public string Name { get; private set; } = name;
    public bool IsStatic { get; private set; } = isStatic;
    public bool IsConstructor { get; private set; } = isConstructor;
    public MetaType? ReturnType { get; set; } = returnType;
    public bool IsArray { get; private set; } = returnsArray;
    public bool IsNative { get; private set; } = isNative;
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
    
    public void ValidateArguments(MetaInstance[] arguments)
    {
        //Verifico que los argumentos esten alineados con los parametros
        if (arguments.Length != Parameters.Count)
            throw new DomainException($"Method {Name} expects {Parameters.Count} arguments, but {arguments.Length} were provided.");

        //Verifico que los tipos de los argumentos coincidan con los tipos de los parametros
        for (int i = 0; i < arguments.Length; i++)
        {
            var expectedType = Parameters[i].Type;
            var actualType = arguments[i].Type;
            if (expectedType.Guid != actualType.Guid)
                throw new DomainException($"Argument {i + 1} of method {Name} expects type {expectedType.Name}, but type {actualType.Name} was provided.");
        }
    }
}