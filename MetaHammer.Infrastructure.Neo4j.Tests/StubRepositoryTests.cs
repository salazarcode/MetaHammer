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

    // Fake minimalista de INeo4JDataAccess que permite configurar resultados de lectura/escritura
    private class FakeNeo4JDataAccess : INeo4JDataAccess
    {
        // Valores que el test puede establecer antes de llamar al repositorio
        public object? NextReadResult { get; set; }
        public object? NextWriteResult { get; set; }

        public Task<T> ExecuteReadAsync<T>(Func<IAsyncQueryRunner, Task<T>> query, CancellationToken cancellationToken = default)
        {
            if (NextReadResult is T t) return Task.FromResult(t);
            return Task.FromResult(default(T)!);
        }

        public Task<T> ExecuteWriteAsync<T>(Func<IAsyncQueryRunner, Task<T>> query, CancellationToken cancellationToken = default)
        {
            if (NextWriteResult is T t) return Task.FromResult(t);
            return Task.FromResult(default(T)!);
        }

        public Task ExecuteWriteAsync(Func<IAsyncQueryRunner, Task> query, CancellationToken cancellationToken = default)
        {
            // Completa sin hacer nada
            return Task.CompletedTask;
        }

        public Task<bool> VerifyConnectivityAsync(CancellationToken cancellationToken = default) => Task.FromResult(true);
        public void Dispose() { }
    }

    // Implementación concreta de RepositoryBase para Stub
    private class StubRepository : RepositoryBase<Stub, Guid>
    {
        public StubRepository(INeo4JDataAccess dataAccess, Microsoft.Extensions.Logging.ILogger logger) : base(dataAccess, logger)
        {
        }

        protected override string NodeLabel => "Stub";

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

    [Fact]
    public async Task CreateAsync_ReturnsEntity_FromDataAccess_Fake()
    {
        var fake = new FakeNeo4JDataAccess();
        var logger = new FakeLogger();
        var repo = new StubRepository(fake, logger);

        var expected = new Stub { Guid = Guid.NewGuid(), Name = "created" };
        fake.NextWriteResult = expected;

        var created = await repo.CreateAsync(expected);

        Assert.NotNull(created);
        Assert.Equal(expected.Guid, created.Guid);
        Assert.Equal(expected.Name, created.Name);
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

        Neo4JOptions fileOptions = null;
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

        // GetById
        var got = await repo.GetByIdAsync(entity.Guid);
        Assert.NotNull(got);
        Assert.Equal(entity.Guid, got!.Guid);

        // Delete
        var deleted = await repo.DeleteAsync(entity.Guid);
        Assert.True(deleted);
    }
}
