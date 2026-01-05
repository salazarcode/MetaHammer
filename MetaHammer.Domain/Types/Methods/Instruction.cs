using MetaHammer.Domain.Common;
using MetaHammer.Domain.Exceptions;
using MetaHammer.Domain.Instances.Base;

namespace MetaHammer.Domain.Types.Methods;

public class Instruction : Entity
{
    public Instruction(Method method, int order, Dictionary<Parameter, Instance> arguments) : base(System.Guid.NewGuid())
    {
        Method = method;
        Order = order;

        foreach (var param in method.GetParameters())
        {
            if (!arguments.ContainsKey(param))
                throw new DomainException($"Parameter {param.Name} from method {method.Name} does not exist in argument list.");
        }

        Arguments = arguments.Select(x => new Argument(x.Key, x.Value)).ToList();
    }
    public Method Method { get; private set; }
    public int Order { get; private set; }
    private List<Argument> Arguments { get; set; }
    public IReadOnlyCollection<Argument> GetArguments => Arguments.AsReadOnly();
}