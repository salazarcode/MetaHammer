using MetaHammer.Domain.Entities;
using MetaHammer.Domain.Entities.Abstract;
using MetaHammer.Domain.Entities.Interfaces;
using MetaHammer.Domain.Entities.Types;

namespace MetaHammer.Domain.Interfaces.Repositories;

/// <summary>
/// Repositorio para la gestión completa de tipos (BaseType y sus derivados),
/// incluyendo sus propiedades, relaciones y versionado.
/// </summary>
public interface ITypeRepository : IRepository<BaseType, Guid>
{
    // ===================================================================
    // BÚSQUEDA Y FILTRADO
    // ===================================================================

    /// <summary>
    /// Obtiene un tipo por su nombre único.
    /// </summary>
    /// <param name="name">Nombre del tipo a buscar.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>El tipo encontrado o null si no existe.</returns>
    Task<BaseType?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca tipos cuyo nombre contenga el término de búsqueda.
    /// </summary>
    /// <param name="searchTerm">Término de búsqueda parcial.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Lista de tipos que coinciden con la búsqueda.</returns>
    Task<IEnumerable<BaseType>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default);

    // ===================================================================
    // CONSULTAS POR CATEGORÍA DE TIPO
    // ===================================================================

    /// <summary>
    /// Obtiene todos los tipos complejos (ComplexType).
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Lista de todos los ComplexType.</returns>
    Task<IEnumerable<ComplexType>> GetAllComplexTypesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene todos los tipos primitivos (PrimitiveType).
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Lista de todos los PrimitiveType.</returns>
    Task<IEnumerable<PrimitiveType>> GetAllPrimitiveTypesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene todos los tipos de objetos de valor (ValueObjectType).
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Lista de todos los ValueObjectType.</returns>
    Task<IEnumerable<ValueObjectType>> GetAllValueObjectTypesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene todos los tipos que pueden ser usados como tipos de propiedades (IPropertyType).
    /// Incluye PrimitiveType y ValueObjectType.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Lista de tipos que implementan IPropertyType.</returns>
    Task<IEnumerable<IPropertyType>> GetUsablePropertyTypesAsync(CancellationToken cancellationToken = default);

    // ===================================================================
    // CONSULTAS ESPECÍFICAS POR TIPO
    // ===================================================================

    /// <summary>
    /// Obtiene un ComplexType específico por su ID.
    /// </summary>
    /// <param name="id">Identificador único del ComplexType.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>El ComplexType encontrado o null si no existe o no es de ese tipo.</returns>
    Task<ComplexType?> GetComplexTypeByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene un PrimitiveType específico por su ID.
    /// </summary>
    /// <param name="id">Identificador único del PrimitiveType.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>El PrimitiveType encontrado o null si no existe o no es de ese tipo.</returns>
    Task<PrimitiveType?> GetPrimitiveTypeByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene un ValueObjectType específico por su ID.
    /// </summary>
    /// <param name="id">Identificador único del ValueObjectType.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>El ValueObjectType encontrado o null si no existe o no es de ese tipo.</returns>
    Task<ValueObjectType?> GetValueObjectTypeByIdAsync(Guid id, CancellationToken cancellationToken = default);

    // ===================================================================
    // GESTIÓN DE HERENCIA (ComplexType y ValueObjectType)
    // ===================================================================

    /// <summary>
    /// Establece una relación de herencia entre dos tipos.
    /// </summary>
    /// <param name="childId">ID del tipo hijo que heredará.</param>
    /// <param name="parentId">ID del tipo padre del que se hereda.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    Task AddParentAsync(Guid childId, Guid parentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Elimina una relación de herencia entre dos tipos.
    /// </summary>
    /// <param name="childId">ID del tipo hijo.</param>
    /// <param name="parentId">ID del tipo padre.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    Task RemoveParentAsync(Guid childId, Guid parentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene todos los tipos padre directos de un tipo.
    /// </summary>
    /// <param name="typeId">ID del tipo.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Lista de tipos padre directos.</returns>
    Task<IEnumerable<BaseType>> GetParentsAsync(Guid typeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene todos los tipos hijo directos de un tipo.
    /// </summary>
    /// <param name="typeId">ID del tipo.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Lista de tipos hijo directos.</returns>
    Task<IEnumerable<BaseType>> GetChildrenAsync(Guid typeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene toda la jerarquía de tipos padre (directos e indirectos) de un tipo.
    /// </summary>
    /// <param name="typeId">ID del tipo.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Lista de todos los tipos ancestros en la jerarquía.</returns>
    Task<IEnumerable<BaseType>> GetAncestorsAsync(Guid typeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene toda la jerarquía de tipos hijo (directos e indirectos) de un tipo.
    /// </summary>
    /// <param name="typeId">ID del tipo.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Lista de todos los tipos descendientes en la jerarquía.</returns>
    Task<IEnumerable<BaseType>> GetDescendantsAsync(Guid typeId, CancellationToken cancellationToken = default);

    // ===================================================================
    // GESTIÓN DE PROPERTIES (ComplexType y ValueObjectType)
    // ===================================================================

    /// <summary>
    /// Agrega una nueva propiedad a un tipo.
    /// </summary>
    /// <param name="typeId">ID del tipo al que se agregará la propiedad.</param>
    /// <param name="property">La propiedad a agregar.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    Task AddPropertyAsync(Guid typeId, Property property, CancellationToken cancellationToken = default);

    /// <summary>
    /// Elimina una propiedad de un tipo por su nombre.
    /// </summary>
    /// <param name="typeId">ID del tipo del que se eliminará la propiedad.</param>
    /// <param name="propertyName">Nombre de la propiedad a eliminar.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    Task RemovePropertyAsync(Guid typeId, string propertyName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene todas las propiedades de un tipo (sin incluir las heredadas).
    /// </summary>
    /// <param name="typeId">ID del tipo.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Lista de propiedades definidas directamente en el tipo.</returns>
    Task<IEnumerable<Property>> GetPropertiesAsync(Guid typeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene todas las propiedades de un tipo, incluyendo las heredadas de sus padres.
    /// </summary>
    /// <param name="typeId">ID del tipo.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Lista de todas las propiedades (propias y heredadas).</returns>
    Task<IEnumerable<Property>> GetAllPropertiesIncludingInheritedAsync(Guid typeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Actualiza una propiedad existente de un tipo.
    /// </summary>
    /// <param name="typeId">ID del tipo.</param>
    /// <param name="propertyName">Nombre de la propiedad a actualizar.</param>
    /// <param name="updatedProperty">La propiedad con los valores actualizados.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    Task UpdatePropertyAsync(Guid typeId, string propertyName, Property updatedProperty, CancellationToken cancellationToken = default);

    // ===================================================================
    // GESTIÓN DE RELATIONS (solo ComplexType)
    // ===================================================================

    /// <summary>
    /// Agrega una nueva relación a un ComplexType.
    /// </summary>
    /// <param name="complexTypeId">ID del ComplexType al que se agregará la relación.</param>
    /// <param name="relation">La relación a agregar.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    Task AddRelationAsync(Guid complexTypeId, Relation relation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Elimina una relación de un ComplexType por su nombre.
    /// </summary>
    /// <param name="complexTypeId">ID del ComplexType del que se eliminará la relación.</param>
    /// <param name="relationName">Nombre de la relación a eliminar.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    Task RemoveRelationAsync(Guid complexTypeId, string relationName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene todas las relaciones de un ComplexType (sin incluir las heredadas).
    /// </summary>
    /// <param name="complexTypeId">ID del ComplexType.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Lista de relaciones definidas directamente en el ComplexType.</returns>
    Task<IEnumerable<Relation>> GetRelationsAsync(Guid complexTypeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene todas las relaciones de un ComplexType, incluyendo las heredadas de sus padres.
    /// </summary>
    /// <param name="complexTypeId">ID del ComplexType.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Lista de todas las relaciones (propias y heredadas).</returns>
    Task<IEnumerable<Relation>> GetAllRelationsIncludingInheritedAsync(Guid complexTypeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Actualiza una relación existente de un ComplexType.
    /// </summary>
    /// <param name="complexTypeId">ID del ComplexType.</param>
    /// <param name="relationName">Nombre de la relación a actualizar.</param>
    /// <param name="updatedRelation">La relación con los valores actualizados.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    Task UpdateRelationAsync(Guid complexTypeId, string relationName, Relation updatedRelation, CancellationToken cancellationToken = default);

    // ===================================================================
    // VERSIONADO (ComplexType y ValueObjectType)
    // ===================================================================

    /// <summary>
    /// Crea una nueva versión de un tipo versionable.
    /// </summary>
    /// <param name="typeId">ID del tipo del que se creará una nueva versión.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>El nuevo tipo con la versión incrementada.</returns>
    Task<BaseType> CreateNewVersionAsync(Guid typeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene el historial completo de versiones de un tipo.
    /// </summary>
    /// <param name="typeId">ID del tipo (cualquier versión).</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Lista ordenada de todas las versiones del tipo (de la más antigua a la más reciente).</returns>
    Task<IEnumerable<BaseType>> GetVersionHistoryAsync(Guid typeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene una versión específica de un tipo.
    /// </summary>
    /// <param name="typeId">ID del tipo (cualquier versión).</param>
    /// <param name="version">Número de versión deseado.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>El tipo en la versión específica o null si no existe.</returns>
    Task<BaseType?> GetVersionAsync(Guid typeId, int version, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene la versión más reciente de un tipo.
    /// </summary>
    /// <param name="typeId">ID del tipo (cualquier versión).</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>La versión más reciente del tipo.</returns>
    Task<BaseType?> GetLatestVersionAsync(Guid typeId, CancellationToken cancellationToken = default);

    // ===================================================================
    // VALIDACIÓN Y VERIFICACIÓN DE USO
    // ===================================================================

    /// <summary>
    /// Verifica si un tipo está siendo usado por otros tipos como propiedad o relación.
    /// </summary>
    /// <param name="typeId">ID del tipo a verificar.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>True si el tipo está siendo usado, false en caso contrario.</returns>
    Task<bool> IsTypeInUseAsync(Guid typeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene todos los tipos que están usando un tipo específico (como propiedad o relación).
    /// </summary>
    /// <param name="typeId">ID del tipo usado.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Lista de tipos que usan el tipo especificado.</returns>
    Task<IEnumerable<BaseType>> GetTypesUsingTypeAsync(Guid typeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica si existe un ciclo en la jerarquía de herencia.
    /// </summary>
    /// <param name="childId">ID del tipo hijo.</param>
    /// <param name="parentId">ID del tipo padre propuesto.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>True si agregar esta relación de herencia crearía un ciclo, false en caso contrario.</returns>
    Task<bool> WouldCreateInheritanceCycleAsync(Guid childId, Guid parentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Valida que un tipo puede ser eliminado de forma segura.
    /// </summary>
    /// <param name="typeId">ID del tipo a validar.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>True si el tipo puede ser eliminado sin violar integridad referencial, false en caso contrario.</returns>
    Task<bool> CanBeDeletedSafelyAsync(Guid typeId, CancellationToken cancellationToken = default);
}