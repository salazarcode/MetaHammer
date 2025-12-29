namespace MetaHammer.Domain.Types;

public class RelationDefinition
{
    Guid Guid { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsArray { get; set; } = false;
    public bool IsComposition { get; set; } = false;
    public ComplexType Type { get; set; } = null!;
}