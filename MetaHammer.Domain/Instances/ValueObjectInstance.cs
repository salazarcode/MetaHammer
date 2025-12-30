using MetaHammer.Domain.Instances.Abstract;
using MetaHammer.Domain.Instances.Interfaces;
using MetaHammer.Domain.Types;

namespace MetaHammer.Domain.Instances;

/// <summary>
/// Instancia de un objeto de valor (Value Object).
/// Es inmutable, se compara por valor y puede usarse como propiedad de otras instancias.
/// Implementa <see cref="IInstanceProperty"/> para poder ser asignado como valor de propiedad.
/// </summary>
public class ValueObjectInstance : MetaInstanceWithProperties, IInstanceProperty
{
    /// <summary>
    /// Crea una nueva instancia de objeto de valor.
    /// </summary>
    /// <param name="type">Tipo ValueObject del cual se crea la instancia.</param>
    public ValueObjectInstance(ValueObjectType type)
    {
        Type = type;
    }
}
