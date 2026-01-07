using MetaHammer.Domain.Common;
using MetaHammer.Domain.Exceptions;
using MetaHammer.Domain.Types.Base;
using MetaHammer.Domain.Types.Entities;
using MetaHammer.Domain.Types.Interfaces;

namespace MetaHammer.Domain.Types;

public class ComplexMetaType(Guid guid, string name, bool isNative = false): MetaTypeWithProperties(guid, name), IMetaType
{
    #region Relations

    private List<MetaRelation> _relations { get; set; } = new();
    
    public IReadOnlyCollection<MetaRelation> Relations => _relations.AsReadOnly();

    public void AddRelation(string name, ComplexMetaType relationType, bool isArray = false, bool isComposition = true)
    {
        var relation = _relations.FirstOrDefault(r => r.Name == name && r.MetaType.Name == relationType.Name);
        
        if(relation == null)
        {
            _relations.Add(new MetaRelation(name, relationType, isArray, isComposition));
        }
        else
        {
            throw new DomainException($"La relación con nombre '{name}' y tipo {relationType.Name} ya existe en el tipo '{this.Name}'.");
        }
    }
    
    #endregion
}