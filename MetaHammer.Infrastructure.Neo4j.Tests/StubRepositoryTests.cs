using Infrastructure.Repository.Neo4j;
using Infrastructure.Repository.Neo4j.Interfaces;
using Infrastructure.Repository.Neo4j.Repositories.Base;
using Infrastructure.Repository.Neo4j.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neo4j.Driver;
using System.IO;
using System.Text.Json;

namespace MetaHammer.Infrastructure.Neo4j.Tests;

public class StubRepositoryTests
{
    // Entidad simple de prueba
    public class Stub
    {
        public Guid Guid { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    // Fake mínimo que implementa explícitamente Microsoft.Extensions.Logging.ILogger (no genérico)
    private class FakeLogger : Microsoft.Extensions.Logging.ILogger
    {
        IDisposable? Microsoft.Extensions.Logging.ILogger.BeginScope<TState>(TState state) => null;
        bool Microsoft.Extensions.Logging.ILogger.IsEnabled(LogLevel logLevel) => true;
        void Microsoft.Extensions.Logging.ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            // no-op
        }
    }

    // Fake logger genérico para inyectar en Neo4JDataAccess (implementa ILogger<T>)
    private class FakeLoggerGeneric<T> : Microsoft.Extensions.Logging.ILogger<T>
    {
        IDisposable? Microsoft.Extensions.Logging.ILogger.BeginScope<TState>(TState state) => null;
        bool Microsoft.Extensions.Logging.ILogger.IsEnabled(LogLevel logLevel) => true;
        void Microsoft.Extensions.Logging.ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            // no-op
        }
    }

    // Implementación concreta de RepositoryBase para Stub
    private class StubRepository : RepositoryBase<Stub, Guid>
    {
        public StubRepository(INeo4JDataAccess dataAccess, Microsoft.Extensions.Logging.ILogger logger) : base(dataAccess, logger)
        {
        }

        // Implementación usando múltiples etiquetas (aquí solo una)
        protected override IEnumerable<string> NodeLabels => new[] { "Stub" };

        // Para probar la creación con múltiples etiquetas, sobreescribimos GetNodeLabels
        protected override IEnumerable<string> GetNodeLabels(Stub entity)
        {
            // Siempre añadimos una etiqueta adicional "Extra" para verificar múltiples etiquetas
            return new[] { "Stub", "Extra" };
        }

        // Exponer wrappers públicos para crear/eliminar relaciones (se prueban en integración)
        public Task CreateRelationAsync(Guid fromId, Guid toId, string relationshipType, Dictionary<string, object?> properties, CancellationToken cancellationToken = default)
            => CreateRelationshipAsync(fromId, toId, relationshipType, properties, cancellationToken);

        public Task DeleteRelationAsync(Guid fromId, Guid toId, string relationshipType, CancellationToken cancellationToken = default)
            => DeleteRelationshipAsync(fromId, toId, relationshipType, cancellationToken);

        protected override Stub MapFromNode(INode node)
        {
            // Mapea robustamente propiedades que pueden venir serializadas (string) o en tipos nativos
            var props = node.Properties;

            Guid guidValue = Guid.Empty;
            if (props.ContainsKey("Guid"))
            {
                var raw = props["Guid"];
                if (raw is Guid g) guidValue = g;
                else if (raw is string s && Guid.TryParse(s, out var parsed)) guidValue = parsed;
            }

            string nameValue = string.Empty;
            if (props.ContainsKey("Name"))
            {
                var raw = props["Name"];
                if (raw is string s) nameValue = s;
                else nameValue = raw?.ToString() ?? string.Empty;
            }

            return new Stub
            {
                Guid = guidValue,
                Name = nameValue
            };
        }

        protected override Dictionary<string, object?> MapToParameters(Stub entity)
        {
            return new Dictionary<string, object?>
            {
                { "Guid", entity.Guid.ToString() }, // almacenar como string para compatibilidad
                { "Name", entity.Name }
            };
        }

        protected override Guid GetEntityId(Stub entity) => entity.Guid;
    }

    // Test de integración real usando Neo4JDataAccess contra una instancia local de Neo4j.
    // Variables de entorno opcionales para configuración:
    //  - NEO4J_URI (por defecto bolt://localhost:7687)
    //  - NEO4J_USER (por defecto neo4j)
    //  - NEO4J_PASSWORD (por defecto neo4j)
    //  - NEO4J_DATABASE (opcional)
    [Fact]
    [Trait("Category", "Integration")]
    public async Task Integration_CreateGetDelete_WithRealNeo4j()
    {
        // Primero intentar leer valores desde variables de entorno (si las hay)
        var envUri = Environment.GetEnvironmentVariable("NEO4J_URI");
        var envUser = Environment.GetEnvironmentVariable("NEO4J_USER");
        var envPassword = Environment.GetEnvironmentVariable("NEO4J_PASSWORD");
        var envDatabase = Environment.GetEnvironmentVariable("NEO4J_DATABASE");

        // Ruta a appsettings.json copiado al output por el csproj
        var configPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");

        Neo4JOptions? fileOptions = null;
        if (File.Exists(configPath))
        {
            var json = await File.ReadAllTextAsync(configPath);
            try
            {
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("Neo4j", out var neoElem))
                {
                    fileOptions = JsonSerializer.Deserialize<Neo4JOptions>(neoElem.GetRawText(), new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
            }
            catch
            {
                // ignore parse errors; we'll fallback to env or defaults below
            }
        }

        // Construir opciones finales (env vars tienen prioridad sobre archivo)
        var uri = envUri ?? fileOptions?.Uri ?? "bolt://localhost:7687";
        var user = envUser ?? fileOptions?.User ?? "neo4j";
        var password = envPassword ?? fileOptions?.Password ?? "neo4j";
        var database = envDatabase ?? fileOptions?.Database ?? "neo4j";

        var options = new Neo4JOptions
        {
            Uri = uri,
            User = user,
            Password = password,
            Database = database,
            MaxRetryCount = fileOptions?.MaxRetryCount ?? 1,
            RetryDelayMilliseconds = fileOptions?.RetryDelayMilliseconds ?? 100
        };

        var optWrapper = Options.Create(options);
        var neoLogger = new FakeLoggerGeneric<Neo4JDataAccess>();

        using var dataAccess = new Neo4JDataAccess(optWrapper, neoLogger);

        var repoLogger = new FakeLogger();
        var repo = new StubRepository(dataAccess, repoLogger);

        var entity = new Stub { Guid = Guid.NewGuid(), Name = "integration-test" };

        // Create
        var created = await repo.CreateAsync(entity);
        Assert.NotNull(created);

        // Verificar que el nodo tenga las etiquetas esperadas (Stub y Extra)
        var labels = await dataAccess.ExecuteReadAsync(async tx =>
        {
            var cursor = await tx.RunAsync("MATCH (n) WHERE n.`Guid` = $id RETURN labels(n) AS labs", new { id = entity.Guid.ToString() });
            var record = await cursor.SingleAsync();
            // labels vienen como una lista de strings
            return record["labs"].As<List<object>>().Select(o => o.ToString()).ToList();
        });

        Assert.Contains("Stub", labels);
        Assert.Contains("Extra", labels);

        // GetById
        var got = await repo.GetByIdAsync(entity.Guid);
        Assert.NotNull(got);
        var gotNonNull = got ?? throw new InvalidOperationException("Expected entity returned");
        Assert.Equal(entity.Guid, gotNonNull.Guid);

        // Delete
        var deleted = await repo.DeleteAsync(entity.Guid);
        Assert.True(deleted);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Integration_Update_WithRealNeo4j()
    {
        // Primero intentar leer valores desde variables de entorno (si las hay)
        var envUri = Environment.GetEnvironmentVariable("NEO4J_URI");
        var envUser = Environment.GetEnvironmentVariable("NEO4J_USER");
        var envPassword = Environment.GetEnvironmentVariable("NEO4J_PASSWORD");
        var envDatabase = Environment.GetEnvironmentVariable("NEO4J_DATABASE");

        // Ruta a appsettings.json copiado al output por el csproj
        var configPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");

        Neo4JOptions? fileOptions = null;
        if (File.Exists(configPath))
        {
            var json = await File.ReadAllTextAsync(configPath);
            try
            {
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("Neo4j", out var neoElem))
                {
                    fileOptions = JsonSerializer.Deserialize<Neo4JOptions>(neoElem.GetRawText(), new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
            }
            catch
            {
                // ignore parse errors; we'll fallback to env or defaults below
            }
        }

        // Construir opciones finales (env vars tienen prioridad sobre archivo)
        var uri = envUri ?? fileOptions?.Uri ?? "bolt://localhost:7687";
        var user = envUser ?? fileOptions?.User ?? "neo4j";
        var password = envPassword ?? fileOptions?.Password ?? "neo4j";
        var database = envDatabase ?? fileOptions?.Database ?? "neo4j";

        var options = new Neo4JOptions
        {
            Uri = uri,
            User = user,
            Password = password,
            Database = database,
            MaxRetryCount = fileOptions?.MaxRetryCount ?? 1,
            RetryDelayMilliseconds = fileOptions?.RetryDelayMilliseconds ?? 100
        };

        var optWrapper = Options.Create(options);
        var neoLogger = new FakeLoggerGeneric<Neo4JDataAccess>();

        using var dataAccess = new Neo4JDataAccess(optWrapper, neoLogger);

        var repoLogger = new FakeLogger();
        var repo = new StubRepository(dataAccess, repoLogger);

        var entity = new Stub { Guid = Guid.NewGuid(), Name = "integration-test" };

        // Create
        var created = await repo.CreateAsync(entity);
        Assert.NotNull(created);

        entity.Name = "updated-name";

        // Update
        var updated = await repo.UpdateAsync(entity);
        Assert.NotNull(updated);
        Assert.Equal("updated-name", updated.Name);

        // GetById
        var got = await repo.GetByIdAsync(entity.Guid);
        Assert.NotNull(got);
        var gotNonNull = got ?? throw new InvalidOperationException("Expected entity returned");
        Assert.Equal(entity.Guid, gotNonNull.Guid);
        Assert.Equal("updated-name", got.Name);

        // Delete
        var deleted = await repo.DeleteAsync(entity.Guid);
        Assert.True(deleted);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task Integration_CreateDeleteRelation_WithRealNeo4j()
    {
        // Primero intentar leer valores desde variables de entorno (si las hay)
        var envUri = Environment.GetEnvironmentVariable("NEO4J_URI");
        var envUser = Environment.GetEnvironmentVariable("NEO4J_USER");
        var envPassword = Environment.GetEnvironmentVariable("NEO4J_PASSWORD");
        var envDatabase = Environment.GetEnvironmentVariable("NEO4J_DATABASE");

        // Ruta a appsettings.json copiado al output por el csproj
        var configPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");

        Neo4JOptions? fileOptions = null;
        if (File.Exists(configPath))
        {
            var json = await File.ReadAllTextAsync(configPath);
            try
            {
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("Neo4j", out var neoElem))
                {
                    fileOptions = JsonSerializer.Deserialize<Neo4JOptions>(neoElem.GetRawText(), new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
            }
            catch
            {
                // ignore parse errors; we'll fallback to env or defaults below
            }
        }

        // Construir opciones finales (env vars tienen prioridad sobre archivo)
        var uri = envUri ?? fileOptions?.Uri ?? "bolt://localhost:7687";
        var user = envUser ?? fileOptions?.User ?? "neo4j";
        var password = envPassword ?? fileOptions?.Password ?? "neo4j";
        var database = envDatabase ?? fileOptions?.Database ?? "neo4j";

        var options = new Neo4JOptions
        {
            Uri = uri,
            User = user,
            Password = password,
            Database = database,
            MaxRetryCount = fileOptions?.MaxRetryCount ?? 1,
            RetryDelayMilliseconds = fileOptions?.RetryDelayMilliseconds ?? 100
        };

        var optWrapper = Options.Create(options);
        var neoLogger = new FakeLoggerGeneric<Neo4JDataAccess>();

        using var dataAccess = new Neo4JDataAccess(optWrapper, neoLogger);

        var repoLogger = new FakeLogger();
        var repo = new StubRepository(dataAccess, repoLogger);

        var fromEntity = new Stub { Guid = Guid.NewGuid(), Name = "from-entity" };
        var toEntity = new Stub { Guid = Guid.NewGuid(), Name = "to-entity" };

        // Create nodos
        await repo.CreateAsync(fromEntity);
        await repo.CreateAsync(toEntity);

        // Crear relación
        var relationType = "RELATED_TO";
        var properties = new Dictionary<string, object?> { { "Since", 2023 } };

        await repo.CreateRelationAsync(fromEntity.Guid, toEntity.Guid, relationType, properties);

        // Verificar que la relación fue creada
        var relations = await dataAccess.ExecuteReadAsync(async tx =>
        {
            var cursor = await tx.RunAsync("MATCH (a)-[r]->(b) WHERE a.`Guid` = $fromId AND b.`Guid` = $toId RETURN type(r) AS relType, r AS relObj", new { fromId = fromEntity.Guid.ToString(), toId = toEntity.Guid.ToString() });
            var records = await cursor.ToListAsync();
            return records.Select(r =>
            {
                var relType = r["relType"].As<string>();
                var relObj = r["relObj"].As<IRelationship>();
                var propsDict = relObj.Properties.ToDictionary(kv => kv.Key, kv => kv.Value);
                return new
                {
                    Type = relType,
                    Properties = propsDict
                };
            }).ToList();
        });

        Assert.Single(relations);
        Assert.Equal(relationType, relations[0].Type);
        Assert.True(relations[0].Properties.ContainsKey("Since"));

        // Delete relación
        await repo.DeleteRelationAsync(fromEntity.Guid, toEntity.Guid, relationType);

        // Verificar que la relación fue eliminada
        var relationsAfterDelete = await dataAccess.ExecuteReadAsync(async tx =>
        {
            var cursor = await tx.RunAsync("MATCH (a)-[r]->(b) WHERE a.`Guid` = $fromId AND b.`Guid` = $toId RETURN type(r) AS relType", new { fromId = fromEntity.Guid.ToString(), toId = toEntity.Guid.ToString() });
            var records = await cursor.ToListAsync();
            return records.Select(r => r["relType"].As<string>()).ToList();
        });

        Assert.Empty(relationsAfterDelete);

        // Delete nodos
        var deletedFrom = await repo.DeleteAsync(fromEntity.Guid);
        var deletedTo = await repo.DeleteAsync(toEntity.Guid);
        Assert.True(deletedFrom);
        Assert.True(deletedTo);
    }
}
