using MetaHammer.Domain.Instances.Abstract;

namespace MetaHammer.Interpreter.Interfaces;

public interface INativeMethod
{
    string Name { get; }
    Context? Context { get; }
    
    MetaInstance Run(params MetaInstance[]? arguments);
}