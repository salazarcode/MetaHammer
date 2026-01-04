using MetaHammer.Domain.Instances.Abstract;
using MetaHammer.Interpreter.Interfaces;

namespace MetaHammer.Interpreter.Methods;

public class CreateInstance(Context? context) : INativeMethod
{
    public string Name => "CreateInstance";

    public Context? Context { get; } = context;
        
    public MetaInstance Run(params MetaInstance[]? arguments)
    {
        throw new NotImplementedException();
    }
}