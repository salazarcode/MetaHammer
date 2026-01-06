using MetaHammer.Domain.Instances;

namespace MetaHammer.Interpreter.Interfaces;

public interface INativeMethod
{
    string Name { get; }
    Context? Context { get; }
    
    MetaObject Run(params MetaObject[]? arguments);
}