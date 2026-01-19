using MetaHammer.Domain.Common;
using MetaHammer.Domain.Exceptions;
using MetaHammer.Domain.Features.Classes.Enums;
using MetaHammer.Domain.Validation;

namespace MetaHammer.Domain.Features.Classes;

public class MetaProperty(Guid guid, string name, MetaClass metaClass, bool isCollection = false, bool isComposition = true) : Entity(guid)
{
    public string Name { get; init; } = name;
    public MetaClass MetaClass { get; init; } = metaClass;
    public bool IsCollection { get; init; } = isCollection;
    public bool IsComposition { get; init; } = isComposition;
}