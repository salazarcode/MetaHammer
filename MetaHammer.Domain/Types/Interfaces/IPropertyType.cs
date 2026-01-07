using MetaHammer.Domain.Types.Entities;
using MetaHammer.Domain.Types.Entities.Method;

namespace MetaHammer.Domain.Types.Interfaces;

/// <summary>
/// Contrato que marca los tipos primitivo y valueObject
/// que pueden por tanto ser usados como valor de propiedades
/// </summary>
public interface IPropertyType : IMetaType
{
}