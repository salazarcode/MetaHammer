using MetaHammer.Domain.Types.Abstract;

namespace MetaHammer.Domain.Types.Methods;

public class MethodParameter
{
    public Guid Guid { get; set; }
    public string Name { get; set; } = string.Empty;
    public MetaType Type { get; set; } = null!;
    public bool IsArray { get; set; } = false;
    public int Order { get; set; }
}