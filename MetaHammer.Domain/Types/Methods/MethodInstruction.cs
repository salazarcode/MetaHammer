using MetaHammer.Domain.Exceptions;

namespace MetaHammer.Domain.Types.Methods;

public class MethodInstruction
{
    public Guid Guid { get; set; }
    public Method Method { get; set; }
    public int Order { get; set; }
    private Dictionary<MethodParameter, string> Arguments { get; set; }

    public MethodInstruction(Method method, Dictionary<MethodParameter, string> arguments)
    {
        Guid = Guid.NewGuid();
        Method = method;
        
        foreach (var param in method.Parameters)
        {
            if (!arguments.ContainsKey(param))
                throw new DomainException($"Parameter {param} does not exist in method {method.Name}.");
        }
        
        Arguments = arguments;
    }
}