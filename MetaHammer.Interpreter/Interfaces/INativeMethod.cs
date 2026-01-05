using MetaHammer.Domain.Instances.Base;

namespace MetaHammer.Interpreter.Interfaces;

public interface INativeMethod
{
    string Name { get; }
    Context? Context { get; }
    
    Instance Run(params Instance[]? arguments);
}