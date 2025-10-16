using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Repository.Neo4j.Configuration;
using Infrastructure.Repository.Neo4j.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neo4j.Driver;
using Polly;
using Polly.Retry;

namespace Infrastructure.Repository.Neo4j;

public class Neo4jDataAccess : INeo4jDataAccess
{
    private readonly IDriver _driver;
    private readonly ILogger<Neo4jDataAccess> _logger;
    private readonly Neo4jOptions _options;
    private readonly AsyncRetryPolicy _retryPolicy;
    private bool _disposed;

    public Neo4jDataAccess(IOptions<Neo4jOptions> options, ILogger<Neo4jDataAccess> logger)
    {
        _options = options.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _driver = GraphDatabase.Driver(_options.Uri, AuthTokens.Basic(_options.User, _options.Password), config => config
            .WithMaxConnectionPoolSize(_options.MaxConnectionPoolSize)
            .WithConnectionIdleTimeout(TimeSpan.FromSeconds(_options.ConnectionIdleTimeoutSeconds))
            .WithConnectionAcquisitionTimeout(TimeSpan.FromSeconds(_options.ConnectionAcquisitionTimeoutSeconds))
            .WithMaxConnectionLifetime(TimeSpan.FromSeconds(_options.MaxConnectionLifetimeSeconds))
        );

        _retryPolicy = Policy
            .Handle<TransientException>() // O excepciones específicas de Neo4j que indiquen transitoriedad
            .WaitAndRetryAsync(
                _options.MaxRetryCount,
                retryAttempt => TimeSpan.FromMilliseconds(_options.RetryDelayMilliseconds * Math.Pow(2, retryAttempt)),
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    _logger.LogWarning(exception, "Retry {RetryCount} for Neo4j operation after {Delay}ms.", retryCount, timeSpan.TotalMilliseconds);
                });
    }

    public async Task<T> ExecuteReadAsync<T>(Func<IAsyncQueryRunner, Task<T>> query, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        await using var session = _driver.AsyncSession(o => o.WithDatabase(_options.Database));
        
        try
        {
            var result = await _retryPolicy.ExecuteAsync(ct => session.ExecuteReadAsync(query), cancellationToken);
            stopwatch.Stop();
            _logger.LogInformation("Read query executed in {ElapsedMilliseconds}ms.", stopwatch.ElapsedMilliseconds);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing read query.");
            throw; 
        }
    }

    public async Task<T> ExecuteWriteAsync<T>(Func<IAsyncQueryRunner, Task<T>> query, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        await using var session = _driver.AsyncSession(o => o.WithDatabase(_options.Database));
        
        try
        {
            var result = await _retryPolicy.ExecuteAsync(ct => session.ExecuteWriteAsync(query), cancellationToken);
            stopwatch.Stop();
            _logger.LogInformation("Write query executed in {ElapsedMilliseconds}ms.", stopwatch.ElapsedMilliseconds);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing write query.");
            throw;
        }
    }
    
    // --- IMPLEMENTACIONES COMPLETADAS ---

    /// <summary>
    /// Ejecuta una operación de escritura que no retorna un valor.
    /// Lo implementa llamando a la versión que sí retorna, pero envolviendo la acción
    /// para que devuelva un valor dummy (en este caso, 'true').
    /// </summary>
    public async Task ExecuteWriteAsync(Func<IAsyncQueryRunner, Task> query, CancellationToken cancellationToken = default)
    {
       await ExecuteWriteAsync(async tx =>
       {
           await query(tx);
           return true; // Retorna un valor dummy para satisfacer la firma del método genérico.
       }, cancellationToken);
    }

    /// <summary>
    /// Verifica la conectividad con la base de datos Neo4j.
    /// Es ideal para ser usado en Health Checks de ASP.NET Core.
    /// </summary>
    public async Task<bool> VerifyConnectivityAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // El driver tiene un método incorporado para esto.
            // No necesita una sesión ni una consulta Cypher.
            await _driver.VerifyConnectivityAsync();
            _logger.LogInformation("Neo4j connectivity verified successfully.");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to verify Neo4j connectivity.");
            return false;
        }
    }

    /// <summary>
    /// Implementación del patrón IDisposable para liberar correctamente los recursos del driver.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            // Libera los recursos gestionados (el driver de Neo4j)
            _logger.LogInformation("Disposing Neo4j driver.");
            _driver?.Dispose();
        }

        // Aquí se podrían liberar recursos no gestionados si los hubiera.

        _disposed = true;
    }
}