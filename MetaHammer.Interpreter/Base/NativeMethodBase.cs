using MetaHammer.Domain.Instances.Base;
using MetaHammer.Interpreter.Interfaces;

namespace MetaHammer.Interpreter.Base;

public abstract class NativeMethodBase : INativeMethod
{
    public string Name { get; } = string.Empty;
    public Context Context { get; }

    public NativeMethodBase(Context context)
    {
        Context = context;
    }
    
    public Instance Run(params Instance[]? arguments)
    {
        throw new NotImplementedException();
    }
}