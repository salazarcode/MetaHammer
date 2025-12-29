namespace MetaHammer.Domain.Types.Abstract;

public abstract class MetaType
{
    public Guid Guid { get; set; }
    public string Name { get; set; } =  string.Empty;
    public int Version { get; set; }
}