using MetaHammer.Domain.Instances.Base;
using MetaHammer.Interpreter.Interfaces;

namespace MetaHammer.Interpreter.Methods;

public class CreateInstance(Context? context) : INativeMethod
{
    public string Name => "CreateInstance";

    public Context? Context { get; } = context;
        
    public Instance Run(params Instance[]? arguments)
    {
        throw new NotImplementedException();
    }
}