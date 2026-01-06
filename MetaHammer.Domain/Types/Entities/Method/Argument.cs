using MetaHammer.Domain.Common;

namespace MetaHammer.Domain.Types.Entities.Method;

public class Argument(Parameter parameter, string variableNameFromContext) : Entity(Guid.NewGuid())
{
    public Parameter Parameter { get; private set; } = parameter;
    public string VariableNameFromContext { get; private set; } = variableNameFromContext;
}