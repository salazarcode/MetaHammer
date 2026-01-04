using MetaHammer.Domain.Common;

namespace MetaHammer.Domain.Types.Methods;

public class Argument(Parameter parameter, string contextVariable) : Entity(Guid.NewGuid())
{
    public Parameter Parameter { get; private set; } = parameter;
    public string ContextVariable { get; private set; } = contextVariable;
}