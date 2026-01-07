using MetaHammer.Domain.Exceptions;
using MetaHammer.Domain.Types.Entities;
using MetaHammer.Domain.Types.Interfaces;

namespace MetaHammer.Domain.Types.Base;

public abstract class MetaTypeWithProperties(Guid guid, string name)  : MetaType(guid, name)
{
    #region Properties
    private List<MetaProperty> _properties { get; set; } = new();
    
    public IReadOnlyCollection<MetaProperty> Properties => _properties.AsReadOnly();

    public void AddProperty(string name, IPropertyType propertyType, bool isArray = false)
    {

        var property = _properties.FirstOrDefault(p => p.Name == name && p.MetaType.Name == propertyType.Name);
        
        if(property == null)
        {
            _properties.Add(new MetaProperty(name, propertyType, isArray));
        }
        else
        {
            throw new DomainException($"La propiedad con nombre '{name}' y tipo {property.MetaType.Name} ya existe en el tipo '{this.Name}'.");
        }
    }
    #endregion
}