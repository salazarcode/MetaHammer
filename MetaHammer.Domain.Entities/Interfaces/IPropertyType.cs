namespace MetaHammer.Domain.Entities.Interfaces;

/// <summary>
/// Interfaz marcadora que restringe qué tipos pueden ser usados como una 'Property'.
/// Solo los tipos que representan un valor (Primitivos y ValueObjects)
/// pueden ser propiedad intrínseca de otro objeto.
/// </summary>
public interface IPropertyType { }