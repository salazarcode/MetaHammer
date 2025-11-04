namespace MetaHammer.Domain.Entities.Abstract;

/// <summary>
/// Clase base abstracta para todos los tipos en el metamodelo.
/// Proporciona una identidad única (Guid) y un nombre (Name).
/// </summary>
public abstract class BaseType
{
    public Guid Guid { get; set; }
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Constructor para nuevos tipos, genera un Guid.
    /// </summary>
    protected BaseType()
    {
        Guid = Guid.NewGuid();
    }

    /// <summary>
    /// Constructor para hidratar tipos desde la persistencia.
    /// </summary>
    protected BaseType(Guid guid)
    {
        Guid = guid;
    }
}