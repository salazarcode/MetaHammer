/*
namespace MetaHammer.Persistence.PostgreSQL.Entities;

/// <summary>
/// Representación en base de datos de un MetaObject (Instancia).
/// Contiene los datos reales.
/// </summary>
public class MetaObjectDM
{
    public Guid Id { get; set; }

    // FK: Necesitamos saber de qué Tipo es este objeto para validar su esquema
    public Guid MetaTypeId { get; set; }
    public MetaTypeDM MetaType { get; set; } = null!;

    public Guid TenantId { get; set; }

    // --- Datos (JSON) ---

    // Valores de las propiedades definidas en el MetaType
    // Ej: { "Age": 30, "Name": "Juan" }
    public string Properties { get; set; } = "{}";

    // Referencias a otros objetos (Solo GUIDs)
    // Ej: { "Orders": ["guid-1", "guid-2"], "CreatedBy": "guid-user" }
    public string Relations { get; set; } = "{}";
}
*/
