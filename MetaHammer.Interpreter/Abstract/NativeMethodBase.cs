using MetaHammer.Domain.Instances.Abstract;
using MetaHammer.Interpreter.Interfaces;

namespace MetaHammer.Interpreter.NativeMethods.Abstract;

public abstract class NativeMethodBase : INativeMethod
{
    public string Name { get; } = string.Empty;
    public Context Context { get; }

    public NativeMethodBase(Context context)
    {
        Context = context;
    }
    
    public MetaInstance Run(params MetaInstance[]? arguments)
    {
        throw new NotImplementedException();
    }
}