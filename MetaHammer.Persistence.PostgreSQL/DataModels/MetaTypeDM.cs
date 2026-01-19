/*
using System.ComponentModel.DataAnnotations;

namespace MetaHammer.Persistence.PostgreSQL.Entities;

/// <summary>
/// Representación en base de datos de un MetaType.
/// Define la estructura (blueprint).
/// </summary>
public class MetaTypeDM
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public Guid TenantId { get; set; }

    public int Version { get; set; }

    // --- Definiciones Estructurales (JSON) ---

    // Contiene la lista de propiedades: Nombre, Tipo de Dato, Validaciones
    // Ej: [{"Name": "Age", "Type": "Integer", "IsRequired": true}]
    public string MetaProperties { get; set; } = "[]";

    // Contiene la definición de relaciones: Nombre, Cardinalidad, Tipo Destino
    // Ej: [{"Name": "Orders", "TargetTypeId": "...", "Type": "OneToMany"}]
    public string MetaRelations { get; set; } = "[]";
}
*/
