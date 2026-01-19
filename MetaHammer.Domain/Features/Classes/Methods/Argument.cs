using MetaHammer.Domain.Common;

namespace MetaHammer.Domain.Features.Classes.Methods;

public class Argument(MetaParameter metaParameter, string variableNameFromContext) : Entity(Guid.NewGuid())
{
    public MetaParameter MetaParameter { get; private set; } = metaParameter;
    public string VariableNameFromContext { get; private set; } = variableNameFromContext;
}