# Guía Completa de Infraestructura Neo4j - Para Desarrolladores Junior

## Tabla de Contenidos
1. [Introducción](#introducción)
2. [Arquitectura General](#arquitectura-general)
3. [Clase Neo4JDataAccess - La Puerta de Entrada a Neo4j](#clase-neo4jdataaccess)
4. [Clase BaseRepository - Tu Repositorio Genérico](#clase-baserepository)
5. [Configuración con Neo4JOptions](#configuración-con-neo4joptions)
6. [Cómo Crear Tu Propio Repositorio](#cómo-crear-tu-propio-repositorio)
7. [Mejores Prácticas](#mejores-prácticas)
8. [Preguntas Frecuentes](#preguntas-frecuentes)

---

## Introducción

### ¿Qué es esto?
Este proyecto es la **capa de infraestructura** que conecta tu aplicación con una base de datos **Neo4j** (una base de datos de grafos). Piensa en esta capa como un "traductor" entre tu código C# y la base de datos.

### ¿Por qué necesitamos esto?
Imagina que estás en España y quieres hablar con alguien en Japón que solo habla japonés. Necesitas un traductor, ¿verdad? Esta capa es ese traductor:
- **Tu aplicación** habla "C# y objetos"
- **Neo4j** habla "Cypher" (el lenguaje de consultas de Neo4j)
- **Esta capa** traduce entre ambos

---

## Arquitectura General

```
┌─────────────────────────────────────────────────────────────┐
│                    TU APLICACIÓN                            │
│              (Servicios, Casos de Uso)                      │
└────────────────────────┬────────────────────────────────────┘
                         │
                         │ Usa
                         ▼
┌─────────────────────────────────────────────────────────────┐
│              REPOSITORIOS ESPECÍFICOS                       │
│         (TypeRepository, UserRepository, etc.)              │
│                                                             │
│          Heredan de ──▶ BaseRepository<T, TId>             │
└────────────────────────┬────────────────────────────────────┘
                         │
                         │ Usa
                         ▼
┌─────────────────────────────────────────────────────────────┐
│                 Neo4JDataAccess                             │
│          (Gestiona conexiones y transacciones)              │
└────────────────────────┬────────────────────────────────────┘
                         │
                         │ Se conecta a
                         ▼
┌─────────────────────────────────────────────────────────────┐
│                   BASE DE DATOS NEO4J                       │
│                  (Almacena los grafos)                      │
└─────────────────────────────────────────────────────────────┘
```

### Componentes Principales

1. **INeo4JDataAccess / Neo4JDataAccess**: La clase que maneja conexiones y transacciones con Neo4j
2. **IRepository / BaseRepository**: La clase base para todos tus repositorios
3. **Neo4JOptions**: Configuración de conexión

---

## Clase Neo4JDataAccess

📍 **Ubicación**: `Neo4JDataAccess.cs`

### ¿Qué hace?
Esta clase es el **gestor de conexiones** con Neo4j. Es como el recepcionista de un hotel que gestiona todas las habitaciones (conexiones) y se asegura de que todo funcione correctamente.

### Conceptos Importantes

#### 1. **El Driver** (`IDriver _driver`)
```csharp
private readonly IDriver _driver;
```

**¿Qué es?**
El driver es como un "cable de conexión" a Neo4j. Es proporcionado por el paquete oficial `Neo4j.Driver` y gestiona el pool de conexiones.

**Analogía**: Piensa en una empresa de taxis:
- El **Driver** es la compañía de taxis
- Cada **sesión** es un taxi individual que puedes usar
- Cuando terminas, devuelves el taxi al pool para que otros lo usen

#### 2. **La Política de Reintento** (`AsyncRetryPolicy _retryPolicy`)
```csharp
private readonly AsyncRetryPolicy _retryPolicy;
```

**¿Qué es?**
A veces, las operaciones fallan temporalmente (red lenta, base de datos ocupada). La política de reintento usa **Polly** (una librería de resiliencia) para volver a intentar automáticamente.

**Ejemplo del mundo real**:
- Llamas por teléfono y está ocupado
- En lugar de rendirte, esperas un poco y vuelves a llamar
- Lo intentas 3 veces antes de darte por vencido

**Configuración del backoff exponencial**:
```csharp
_retryPolicy = Policy
    .Handle<TransientException>()
    .WaitAndRetryAsync(
        _options.MaxRetryCount,  // Cuántos reintentos (ej: 3)
        retryAttempt => TimeSpan.FromMilliseconds(
            _options.RetryDelayMilliseconds * Math.Pow(2, retryAttempt)
        )
    );
```

Si `RetryDelayMilliseconds = 1000`:
- Intento 1 falla → Espera 2 segundos (1000 * 2^1)
- Intento 2 falla → Espera 4 segundos (1000 * 2^2)
- Intento 3 falla → Espera 8 segundos (1000 * 2^3)

#### 3. **Métodos de Lectura y Escritura**

##### `ExecuteReadAsync<T>`
```csharp
public async Task<T> ExecuteReadAsync<T>(
    Func<IAsyncQueryRunner, Task<T>> query,
    CancellationToken cancellationToken = default)
```

**¿Qué hace?**
Ejecuta una consulta de **SOLO LECTURA** (SELECT en SQL, MATCH en Cypher).

**¿Por qué separar lectura de escritura?**
Neo4j optimiza las consultas de manera diferente:
- **Lecturas**: Pueden ejecutarse en réplicas (servidores secundarios)
- **Escrituras**: Deben ejecutarse en el servidor principal

**Ejemplo de uso**:
```csharp
var user = await _dataAccess.ExecuteReadAsync(async tx =>
{
    var query = "MATCH (u:User {Id: $id}) RETURN u";
    var cursor = await tx.RunAsync(query, new { id = userId });
    var record = await cursor.SingleAsync();
    return record["u"].As<INode>();
});
```

##### `ExecuteWriteAsync<T>`
```csharp
public async Task<T> ExecuteWriteAsync<T>(
    Func<IAsyncQueryRunner, Task<T>> query,
    CancellationToken cancellationToken = default)
```

**¿Qué hace?**
Ejecuta una consulta que **MODIFICA DATOS** (INSERT, UPDATE, DELETE en SQL; CREATE, SET, DELETE en Cypher).

**Ejemplo de uso**:
```csharp
var newUser = await _dataAccess.ExecuteWriteAsync(async tx =>
{
    var query = "CREATE (u:User) SET u = $props RETURN u";
    var cursor = await tx.RunAsync(query, new { props = userData });
    var record = await cursor.SingleAsync();
    return record["u"].As<INode>();
});
```

#### 4. **Verificación de Conectividad**

```csharp
public async Task<bool> VerifyConnectivityAsync(CancellationToken cancellationToken = default)
```

**¿Para qué sirve?**
Este método es perfecto para **Health Checks** (verificaciones de salud) en aplicaciones web.

**Uso típico en ASP.NET Core**:
```csharp
services.AddHealthChecks()
    .AddCheck("neo4j", async () =>
    {
        var isHealthy = await neo4jDataAccess.VerifyConnectivityAsync();
        return isHealthy
            ? HealthCheckResult.Healthy("Neo4j is responsive")
            : HealthCheckResult.Unhealthy("Neo4j is not responding");
    });
```

#### 5. **Patrón IDisposable**

```csharp
public void Dispose()
{
    Dispose(true);
    GC.SuppressFinalize(this);
}
```

**¿Por qué es importante?**
El driver de Neo4j mantiene conexiones abiertas. Si no las cierras correctamente, tendrás **fugas de recursos** (memory leaks).

**Buena práctica**:
```csharp
// Si lo inyectas con Dependency Injection (recomendado)
services.AddSingleton<INeo4JDataAccess, Neo4JDataAccess>();

// El contenedor de DI se encargará de llamar Dispose cuando la app se detenga
```

---

## Clase BaseRepository

📍 **Ubicación**: `Repositories/Base/BaseRepository.cs`

### ¿Qué hace?
`BaseRepository` es una **clase base abstracta** que implementa operaciones CRUD (Create, Read, Update, Delete) comunes para cualquier entidad. Es como una "plantilla" que puedes reutilizar.

### Conceptos Importantes

#### 1. **Genéricos** (`<TEntity, TId>`)

```csharp
public abstract class RepositoryBase<TEntity, TId> : IRepository<TEntity, TId>
    where TEntity : class
```

**¿Qué significa esto?**
- `TEntity`: El tipo de tu entidad (ej: `User`, `Product`, `Type`)
- `TId`: El tipo del identificador (ej: `Guid`, `int`, `string`)

**Ejemplo**:
```csharp
// Repositorio para Users con ID tipo Guid
public class UserRepository : RepositoryBase<User, Guid>
{
    // ...
}

// Repositorio para Products con ID tipo int
public class ProductRepository : RepositoryBase<Product, int>
{
    // ...
}
```

#### 2. **Propiedades Abstractas y Virtuales**

##### `NodeLabels` (Abstracta)
```csharp
protected abstract IEnumerable<string> NodeLabels { get; }
```

**¿Qué es?**
Las **etiquetas** (labels) de Neo4j son como "tipos" o "categorías" de nodos.

**Ejemplo**:
```csharp
// Un nodo simple con una etiqueta
protected override IEnumerable<string> NodeLabels => new[] { "User" };
// En Neo4j: (u:User)

// Un nodo con múltiples etiquetas (herencia/polimorfismo)
protected override IEnumerable<string> NodeLabels => new[] { "Type", "ComplexType" };
// En Neo4j: (t:Type:ComplexType)
```

##### `IdPropertyName` (Virtual)
```csharp
protected virtual string IdPropertyName => "Guid";
```

**¿Qué es?**
El nombre de la propiedad que actúa como identificador único en Neo4j.

**Por defecto es "Guid", pero puedes cambiarlo**:
```csharp
protected override string IdPropertyName => "UserId";
```

#### 3. **Métodos Abstractos (DEBES Implementar)**

##### `MapFromNode`
```csharp
protected abstract TEntity MapFromNode(INode node);
```

**¿Qué hace?**
Convierte un nodo de Neo4j a tu objeto C#.

**Ejemplo de implementación**:
```csharp
protected override User MapFromNode(INode node)
{
    return new User
    {
        Id = Guid.Parse(node.Properties["Guid"].As<string>()),
        Name = node.Properties["Name"].As<string>(),
        Email = node.Properties["Email"].As<string>(),
        CreatedAt = DateTime.Parse(node.Properties["CreatedAt"].As<string>())
    };
}
```

##### `MapToParameters`
```csharp
protected abstract Dictionary<string, object?> MapToParameters(TEntity entity);
```

**¿Qué hace?**
Convierte tu objeto C# a un diccionario para Neo4j.

**Ejemplo de implementación**:
```csharp
protected override Dictionary<string, object?> MapToParameters(User entity)
{
    return new Dictionary<string, object?>
    {
        ["Guid"] = entity.Id.ToString(),
        ["Name"] = entity.Name,
        ["Email"] = entity.Email,
        ["CreatedAt"] = entity.CreatedAt.ToString("O")
    };
}
```

##### `GetEntityId`
```csharp
protected abstract TId GetEntityId(TEntity entity);
```

**¿Qué hace?**
Extrae el ID de tu entidad.

**Ejemplo de implementación**:
```csharp
protected override Guid GetEntityId(User entity)
{
    return entity.Id;
}
```

#### 4. **Operaciones CRUD**

##### `CreateAsync`
```csharp
public virtual async Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
```

**¿Qué hace?**
Crea un nuevo nodo en Neo4j.

**Flujo interno**:
1. Llama a `MapToParameters` para convertir tu entidad a un diccionario
2. Obtiene las etiquetas con `GetNodeLabels`
3. Construye una consulta Cypher CREATE
4. Ejecuta la consulta con `DataAccess.ExecuteWriteAsync`
5. Convierte el resultado con `MapFromNode`

**Consulta Cypher generada** (ejemplo):
```cypher
CREATE (n:`User`)
SET n = $properties
RETURN n
```

##### `GetByIdAsync`
```csharp
public virtual async Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
```

**¿Qué hace?**
Busca un nodo por su ID.

**Consulta Cypher generada** (ejemplo):
```cypher
MATCH (n:User)
WHERE n.`Guid` = $id
RETURN n
```

**Nota**: Devuelve `null` si no encuentra el nodo.

##### `GetAllAsync`
```csharp
public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
```

**¿Qué hace?**
Obtiene todos los nodos de un tipo.

**Consulta Cypher generada** (ejemplo):
```cypher
MATCH (n:User)
RETURN n
```

**⚠️ ADVERTENCIA**: Ten cuidado con este método en producción. Si tienes millones de nodos, esta consulta puede ser muy lenta. Considera implementar paginación.

##### `UpdateAsync`
```csharp
public virtual async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
```

**¿Qué hace?**
Actualiza las propiedades de un nodo existente.

**Consulta Cypher generada** (ejemplo):
```cypher
MATCH (n:User {`Guid`: $id})
SET n.`Name` = $props.`Name`, n.`Email` = $props.`Email`
RETURN n
```

**Detalle importante**: NO actualiza la propiedad ID (se excluye con `Where(k => k != IdPropertyName)`).

##### `DeleteAsync`
```csharp
public virtual async Task<bool> DeleteAsync(TId id, CancellationToken cancellationToken = default)
```

**¿Qué hace?**
Elimina un nodo y TODAS sus relaciones (por eso usa `DETACH DELETE`).

**Consulta Cypher generada** (ejemplo):
```cypher
MATCH (n:User {`Guid`: $id})
DETACH DELETE n
RETURN count(n) as deletedCount
```

**¿Qué es DETACH DELETE?**
- `DELETE n`: Solo eliminaría el nodo si NO tiene relaciones
- `DETACH DELETE n`: Elimina el nodo Y todas sus relaciones (más seguro)

#### 5. **Métodos de Relaciones** (Protegidos)

##### `CreateRelationshipAsync`
```csharp
protected async Task CreateRelationshipAsync(
    Guid fromNodeId,
    Guid toNodeId,
    string relationshipType,
    Dictionary<string, object?> properties,
    CancellationToken cancellationToken = default)
```

**¿Qué hace?**
Crea una relación entre dos nodos.

**Ejemplo de uso en un repositorio especializado**:
```csharp
public async Task AddUserToTeam(Guid userId, Guid teamId)
{
    await CreateRelationshipAsync(
        fromNodeId: userId,
        toNodeId: teamId,
        relationshipType: "MEMBER_OF",
        properties: new Dictionary<string, object?>
        {
            ["JoinedAt"] = DateTime.UtcNow.ToString("O"),
            ["Role"] = "Member"
        }
    );
}
```

**Consulta Cypher generada**:
```cypher
MATCH (from) WHERE from.`Guid` = $fromId
MATCH (to) WHERE to.`Guid` = $toId
CREATE (from)-[r:`MEMBER_OF`]->(to)
SET r = $properties
```

##### `DeleteRelationshipAsync`
```csharp
protected async Task DeleteRelationshipAsync(
    Guid fromNodeId,
    Guid toNodeId,
    string relationshipType,
    CancellationToken cancellationToken = default)
```

**¿Qué hace?**
Elimina una relación específica entre dos nodos.

**Ejemplo de uso**:
```csharp
public async Task RemoveUserFromTeam(Guid userId, Guid teamId)
{
    await DeleteRelationshipAsync(
        fromNodeId: userId,
        toNodeId: teamId,
        relationshipType: "MEMBER_OF"
    );
}
```

#### 6. **Seguridad: Sanitización de Etiquetas**

```csharp
private string SanitizeLabel(string label)
{
    if (string.IsNullOrWhiteSpace(label))
        throw new ArgumentException($"Nombre de etiqueta inválido: '{label}'");

    if (label.Contains('`') || label.Contains(':') || label.Any(char.IsControl))
        throw new ArgumentException($"Nombre de etiqueta inválido: '{label}'");

    var trimmed = label.Trim();
    return $"`{trimmed}`";
}
```

**¿Por qué es importante?**
Esto previene **inyección de Cypher** (similar a inyección SQL).

**Sin sanitización** (PELIGROSO):
```csharp
var label = "User`)-[r:ADMIN]->(a:Admin) CREATE (a"; // ¡Código malicioso!
var query = $"CREATE (n:{label})";
// Resultado: CREATE (n:User`)-[r:ADMIN]->(a:Admin) CREATE (a)
// ¡Esto crearía un usuario admin no autorizado!
```

**Con sanitización** (SEGURO):
```csharp
var label = SanitizeLabel("User`)-[r:ADMIN]->(a");
// Lanza excepción: "Nombre de etiqueta inválido"
```

---

## Configuración con Neo4JOptions

📍 **Ubicación**: `Configuration/Neo4JOptions.cs`

### Propiedades de Configuración

#### Conexión Básica
```csharp
public string Uri { get; set; }      // Ej: "bolt://localhost:7687"
public string User { get; set; }     // Ej: "neo4j"
public string Password { get; set; } // Ej: "tu-password-segura"
public string? Database { get; set; } // Ej: "neo4j" o null para default
```

#### Pool de Conexiones
```csharp
public int MaxConnectionPoolSize { get; set; } = 100;
// Máximo de conexiones simultáneas al servidor
```

**¿Cómo elegir este valor?**
- Aplicación pequeña (pocos usuarios): 20-50
- Aplicación mediana: 100-200
- Aplicación grande: 200-500

#### Timeouts
```csharp
public int ConnectionIdleTimeoutSeconds { get; set; } = 60;
// Tiempo antes de cerrar una conexión inactiva

public int ConnectionAcquisitionTimeoutSeconds { get; set; } = 60;
// Tiempo máximo para esperar una conexión disponible

public int MaxConnectionLifetimeSeconds { get; set; } = 3600;
// Tiempo máximo de vida de una conexión (se renueva después)
```

#### Política de Reintentos
```csharp
public int MaxRetryCount { get; set; } = 3;
// Número de reintentos antes de fallar

public int RetryDelayMilliseconds { get; set; } = 1000;
// Delay base para el backoff exponencial
```

### Configuración en appsettings.json

```json
{
  "Neo4j": {
    "Uri": "bolt://localhost:7687",
    "User": "neo4j",
    "Password": "tu-password",
    "Database": "neo4j",
    "MaxConnectionPoolSize": 100,
    "ConnectionIdleTimeoutSeconds": 60,
    "ConnectionAcquisitionTimeoutSeconds": 60,
    "MaxConnectionLifetimeSeconds": 3600,
    "MaxRetryCount": 3,
    "RetryDelayMilliseconds": 1000,
    "EnableDriverLogging": false
  }
}
```

### Configuración en Program.cs (ASP.NET Core)

```csharp
// 1. Configurar las opciones
builder.Services.Configure<Neo4JOptions>(
    builder.Configuration.GetSection(Neo4JOptions.SectionName)
);

// 2. Validar las opciones al inicio
builder.Services.AddOptions<Neo4JOptions>()
    .BindConfiguration(Neo4JOptions.SectionName)
    .ValidateDataAnnotations()
    .ValidateOnStart();

// 3. Registrar el DataAccess como Singleton
builder.Services.AddSingleton<INeo4JDataAccess, Neo4JDataAccess>();

// 4. Registrar tus repositorios como Scoped
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITypeRepository, TypeRepository>();
```

---

## Cómo Crear Tu Propio Repositorio

### Paso 1: Define tu Entidad de Dominio

```csharp
namespace MetaHammer.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
```

### Paso 2: Define la Interfaz del Repositorio

```csharp
namespace MetaHammer.Domain.Interfaces.Repositories;

public interface IUserRepository : IRepository<User, Guid>
{
    // Métodos adicionales específicos de User
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetUsersCreatedAfterAsync(DateTime date, CancellationToken cancellationToken = default);
}
```

### Paso 3: Implementa el Repositorio

```csharp
using Infrastructure.Repository.Neo4j.Interfaces;
using Infrastructure.Repository.Neo4j.Repositories.Base;
using MetaHammer.Domain.Entities;
using MetaHammer.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using Neo4j.Driver;

namespace MetaHammer.Infrastructure.Repositories;

public class UserRepository : RepositoryBase<User, Guid>, IUserRepository
{
    // Constructor: inyecta dependencias necesarias
    public UserRepository(
        INeo4JDataAccess dataAccess,
        ILogger<UserRepository> logger)
        : base(dataAccess, logger)
    {
    }

    // PASO 1: Define las etiquetas del nodo
    protected override IEnumerable<string> NodeLabels => new[] { "User" };

    // PASO 2: Convierte de Neo4j a tu entidad
    protected override User MapFromNode(INode node)
    {
        return new User
        {
            Id = Guid.Parse(node.Properties["Guid"].As<string>()),
            Name = node.Properties["Name"].As<string>(),
            Email = node.Properties["Email"].As<string>(),
            CreatedAt = DateTime.Parse(node.Properties["CreatedAt"].As<string>())
        };
    }

    // PASO 3: Convierte de tu entidad a parámetros para Neo4j
    protected override Dictionary<string, object?> MapToParameters(User entity)
    {
        return new Dictionary<string, object?>
        {
            ["Guid"] = entity.Id.ToString(),
            ["Name"] = entity.Name,
            ["Email"] = entity.Email,
            ["CreatedAt"] = entity.CreatedAt.ToString("O") // ISO 8601
        };
    }

    // PASO 4: Extrae el ID de tu entidad
    protected override Guid GetEntityId(User entity)
    {
        return entity.Id;
    }

    // PASO 5: Implementa métodos específicos
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var query = @"
            MATCH (u:User)
            WHERE u.Email = $email
            RETURN u";

        return await DataAccess.ExecuteReadAsync(async tx =>
        {
            var cursor = await tx.RunAsync(query, new { email });
            var records = await cursor.ToListAsync(cancellationToken);
            var record = records.FirstOrDefault();
            return record != null ? MapFromNode(record["u"].As<INode>()) : null;
        }, cancellationToken);
    }

    public async Task<IEnumerable<User>> GetUsersCreatedAfterAsync(DateTime date, CancellationToken cancellationToken = default)
    {
        var query = @"
            MATCH (u:User)
            WHERE datetime(u.CreatedAt) > datetime($date)
            RETURN u
            ORDER BY u.CreatedAt DESC";

        return await DataAccess.ExecuteReadAsync(async tx =>
        {
            var cursor = await tx.RunAsync(query, new { date = date.ToString("O") });
            var records = await cursor.ToListAsync(cancellationToken);
            return records.Select(record => MapFromNode(record["u"].As<INode>()));
        }, cancellationToken);
    }
}
```

### Paso 4: Registra en el Contenedor de DI

```csharp
// En Program.cs o Startup.cs
services.AddScoped<IUserRepository, UserRepository>();
```

### Paso 5: Usa el Repositorio

```csharp
public class UserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User> CreateUserAsync(string name, string email)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = email,
            CreatedAt = DateTime.UtcNow
        };

        return await _userRepository.CreateAsync(user);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _userRepository.GetByEmailAsync(email);
    }
}
```

---

## Mejores Prácticas

### 1. Manejo de Errores

```csharp
public async Task<User?> GetUserSafelyAsync(Guid userId)
{
    try
    {
        return await _userRepository.GetByIdAsync(userId);
    }
    catch (Neo4jException ex) when (ex.Code == "Neo.ClientError.Statement.SyntaxError")
    {
        _logger.LogError(ex, "Syntax error in Cypher query");
        throw new InvalidOperationException("Error en la consulta", ex);
    }
    catch (ServiceUnavailableException ex)
    {
        _logger.LogError(ex, "Neo4j service is unavailable");
        throw new InvalidOperationException("Base de datos no disponible", ex);
    }
}
```

### 2. Usa CancellationToken

```csharp
public async Task<IEnumerable<User>> GetAllUsersAsync(CancellationToken cancellationToken)
{
    // El token permite cancelar operaciones largas
    return await _userRepository.GetAllAsync(cancellationToken);
}
```

**Ejemplo en un controlador API**:
```csharp
[HttpGet]
public async Task<IActionResult> GetUsers(CancellationToken cancellationToken)
{
    var users = await _userService.GetAllUsersAsync(cancellationToken);
    return Ok(users);
}
// Si el cliente cancela la petición HTTP, la operación de BD también se cancela
```

### 3. Validación de Datos

```csharp
protected override Dictionary<string, object?> MapToParameters(User entity)
{
    // Valida antes de mapear
    if (string.IsNullOrWhiteSpace(entity.Name))
        throw new ArgumentException("Name cannot be empty");

    if (!IsValidEmail(entity.Email))
        throw new ArgumentException("Invalid email format");

    return new Dictionary<string, object?>
    {
        ["Guid"] = entity.Id.ToString(),
        ["Name"] = entity.Name.Trim(),
        ["Email"] = entity.Email.ToLowerInvariant(),
        ["CreatedAt"] = entity.CreatedAt.ToString("O")
    };
}
```

### 4. Índices en Neo4j

Para mejorar el rendimiento, crea índices en las propiedades que consultas frecuentemente:

```cypher
// Ejecuta esto en Neo4j Browser o en un script de migración
CREATE INDEX user_email_index FOR (u:User) ON (u.Email);
CREATE INDEX user_guid_index FOR (u:User) ON (u.Guid);
```

### 5. Logging Efectivo

```csharp
public async Task<User> CreateAsync(User user)
{
    _logger.LogInformation("Creating user with email: {Email}", user.Email);

    var created = await _userRepository.CreateAsync(user);

    _logger.LogInformation("User created successfully with ID: {UserId}", created.Id);

    return created;
}
```

### 6. Testing

```csharp
public class UserRepositoryTests
{
    private readonly Mock<INeo4JDataAccess> _mockDataAccess;
    private readonly Mock<ILogger<UserRepository>> _mockLogger;
    private readonly UserRepository _repository;

    public UserRepositoryTests()
    {
        _mockDataAccess = new Mock<INeo4JDataAccess>();
        _mockLogger = new Mock<ILogger<UserRepository>>();
        _repository = new UserRepository(_mockDataAccess.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateUser()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "John",
            Email = "john@test.com"
        };

        _mockDataAccess
            .Setup(x => x.ExecuteWriteAsync(
                It.IsAny<Func<IAsyncQueryRunner, Task<User>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _repository.CreateAsync(user);

        // Assert
        Assert.Equal(user.Id, result.Id);
        _mockDataAccess.Verify(x => x.ExecuteWriteAsync(
            It.IsAny<Func<IAsyncQueryRunner, Task<User>>>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
```

---

## Preguntas Frecuentes

### ¿Por qué usar Guid en lugar de int para IDs?

**Ventajas de Guid**:
- No necesitas coordinar la generación de IDs entre servicios distribuidos
- Puedes generar el ID en el cliente antes de guardarlo
- Evitas problemas de concurrencia en generación de IDs

**Desventajas de Guid**:
- Ocupan más espacio (16 bytes vs 4 bytes)
- Son menos legibles para humanos

### ¿Cuándo usar Read vs Write?

- **ExecuteReadAsync**: SELECT, consultas que NO modifican datos
- **ExecuteWriteAsync**: INSERT, UPDATE, DELETE, creación de relaciones

**¿Qué pasa si me equivoco?**
- Si usas Write para una lectura: Funciona, pero es menos eficiente
- Si usas Read para una escritura: **NO FUNCIONARÁ** (lanzará error)

### ¿Cómo manejo relaciones complejas?

```csharp
// En tu repositorio especializado
public class UserRepository : RepositoryBase<User, Guid>, IUserRepository
{
    public async Task AddFriendAsync(Guid userId, Guid friendId, CancellationToken cancellationToken = default)
    {
        // Usa el método protegido de la clase base
        await CreateRelationshipAsync(
            fromNodeId: userId,
            toNodeId: friendId,
            relationshipType: "FRIEND_OF",
            properties: new Dictionary<string, object?>
            {
                ["CreatedAt"] = DateTime.UtcNow.ToString("O")
            },
            cancellationToken
        );
    }

    public async Task<IEnumerable<User>> GetFriendsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var query = @"
            MATCH (u:User {Guid: $userId})-[:FRIEND_OF]->(friend:User)
            RETURN friend";

        return await DataAccess.ExecuteReadAsync(async tx =>
        {
            var cursor = await tx.RunAsync(query, new { userId = userId.ToString() });
            var records = await cursor.ToListAsync(cancellationToken);
            return records.Select(r => MapFromNode(r["friend"].As<INode>()));
        }, cancellationToken);
    }
}
```

### ¿Cómo implemento paginación?

```csharp
public async Task<IEnumerable<User>> GetUsersPaginatedAsync(
    int page,
    int pageSize,
    CancellationToken cancellationToken = default)
{
    var skip = (page - 1) * pageSize;

    var query = @"
        MATCH (u:User)
        RETURN u
        ORDER BY u.CreatedAt DESC
        SKIP $skip
        LIMIT $limit";

    return await DataAccess.ExecuteReadAsync(async tx =>
    {
        var cursor = await tx.RunAsync(query, new { skip, limit = pageSize });
        var records = await cursor.ToListAsync(cancellationToken);
        return records.Select(r => MapFromNode(r["u"].As<INode>()));
    }, cancellationToken);
}
```

### ¿Cómo manejo transacciones?

El `ExecuteWriteAsync` ya maneja transacciones automáticamente. Pero si necesitas múltiples operaciones en una sola transacción:

```csharp
public async Task TransferUserToTeam(Guid userId, Guid oldTeamId, Guid newTeamId)
{
    await DataAccess.ExecuteWriteAsync(async tx =>
    {
        // Todo esto ocurre en UNA transacción

        // 1. Eliminar relación con equipo antiguo
        var deleteQuery = @"
            MATCH (u:User {Guid: $userId})-[r:MEMBER_OF]->(t:Team {Guid: $oldTeamId})
            DELETE r";
        await tx.RunAsync(deleteQuery, new
        {
            userId = userId.ToString(),
            oldTeamId = oldTeamId.ToString()
        });

        // 2. Crear relación con nuevo equipo
        var createQuery = @"
            MATCH (u:User {Guid: $userId})
            MATCH (t:Team {Guid: $newTeamId})
            CREATE (u)-[r:MEMBER_OF {JoinedAt: datetime()}]->(t)";
        await tx.RunAsync(createQuery, new
        {
            userId = userId.ToString(),
            newTeamId = newTeamId.ToString()
        });

        return true; // Commit automático si no hay excepciones
    });
}
```

---

## Recursos Adicionales

### Documentación Oficial
- [Neo4j .NET Driver](https://neo4j.com/docs/dotnet-manual/current/)
- [Cypher Query Language](https://neo4j.com/docs/cypher-manual/current/)
- [Polly (Resiliencia)](https://github.com/App-vNext/Polly)

### Herramientas Útiles
- **Neo4j Browser**: Interfaz web para explorar y consultar tu base de datos
- **Neo4j Desktop**: Aplicación de escritorio para gestionar instancias locales
- **Cypher Shell**: CLI para ejecutar consultas

### Consejos Finales
1. **Empieza simple**: Usa los métodos CRUD básicos antes de escribir consultas complejas
2. **Lee la documentación de Cypher**: Es un lenguaje muy expresivo y poderoso
3. **Usa Neo4j Browser**: Visualizar los grafos te ayuda a entender las consultas
4. **Escribe tests**: Mockea `INeo4JDataAccess` para probar tu lógica sin BD
5. **Monitorea el rendimiento**: Usa los logs y métricas para identificar consultas lentas

---

**¡Bienvenido al mundo de las bases de datos de grafos!** 🚀

Si tienes dudas, revisa el código fuente comentado o consulta con tu equipo.
