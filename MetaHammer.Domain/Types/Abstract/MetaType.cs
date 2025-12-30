using MetaHammer.Domain.Validation;

namespace MetaHammer.Domain.Types.Abstract;

/// <summary>
/// Clase base abstracta para todos los meta-tipos del sistema.
/// Un meta-tipo define la estructura y comportamiento de las instancias que se crean a partir de él.
/// </summary>
public abstract class MetaType
{
    private string _name = string.Empty;

    /// <summary>
    /// Identificador único del tipo.
    /// </summary>
    public Guid Guid { get; protected set; }

    /// <summary>
    /// Nombre del tipo en formato PascalCase (ej: "Person", "OrderLine").
    /// </summary>
    public string Name
    {
        get => _name;
        protected set
        {
            NameFormatValidator.ValidatePascalCase(value, "Type");
            _name = value;
        }
    }

    /// <summary>
    /// Versión del tipo para control de cambios en el esquema.
    /// </summary>
    public int Version { get; protected set; } = 1;
}