namespace MetaHammer.Domain.Types;

using MetaHammer.Domain.Types.Interfaces;

public class PropertyDefinition
{
    Guid Guid { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsArray { get; set; } = false;
    public IPropertyType PropertyType { get; set; } = null!;
}