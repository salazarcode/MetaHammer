using MetaHammer.Domain.Types.Entities.Method;

namespace MetaHammer.Domain.Types.Interfaces;

/// <summary>
/// Contrato común de los tipos que pueden usarse como tipos en los MetaObject
/// </summary>
public interface IMetaType
{
    Guid Guid { get; }
    string Name { get; }
    IReadOnlyCollection<Method> Methods { get; }
}