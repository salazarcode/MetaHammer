namespace MetaHammer.Domain.Types.Interfaces;

public interface ITypeWithProperties
{
    public List<PropertyDefinition> Properties { get; set; }
}