using MetaHammer.Domain.Entities.Abstract;
using MetaHammer.Domain.Entities.Interfaces;

namespace MetaHammer.Domain.Entities.Types;

/// <summary>
/// Representa un tipo de dato primitivo (ej. string, int, boolean, reference).
/// Implementa IPropertyType, por lo que puede ser el tipo de una 'Property'.
/// </summary>
public class PrimitiveType : BaseType, IPropertyType
{
    public PrimitiveType(string name) : base() 
    { 
        Name = name; 
    }

    public PrimitiveType(Guid guid, string name) : base(guid) 
    { 
        Name = name; 
    }
}