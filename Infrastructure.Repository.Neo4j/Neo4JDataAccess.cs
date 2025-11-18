using System.Diagnostics;
using Infrastructure.Repository.Neo4j.Configuration;
using Infrastructure.Repository.Neo4j.Interfaces;
using MetaHammer.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neo4j.Driver;
using Polly;
using Polly.Retry;

namespace Infrastructure.Repository.Neo4j;

/// <summary>
/// Acceso a datos sobre Neo4j con manejo de resiliencia usando Polly.
/// </summary>
/// <remarks>
/// - Usa Neo4j.Driver para comunicaciones con la base de datos.
/// - Aplica una <see cref="_retryPolicy"/> de Polly (WaitAndRetryAsync) para operaciones transitorias.
/// - Las opciones de conexión y de política se leen desde <see cref="Neo4JOptions"/>.
/// </remarks>
public class Neo4JDataAccess : INeo4JDataAccess
{
    /// <summary>
    /// Driver de Neo4j (gestiona conexiones).
    /// </summary>
    private readonly IDriver _driver;

    /// <summary>
    /// Logger para registrar eventos e información de reintentos.
    /// </summary>
    private readonly ILogger<Neo4JDataAccess> _logger;

    /// <summary>
    /// Opciones de configuración inyectadas (URI, credenciales, parámetros de retry).
    /// </summary>
    private readonly Neo4JOptions _options;

    /// <summary>
    /// Política de reintento asíncrona (Polly) utilizada para operaciones de lectura/escritura.
    /// </summary>
    /// <remarks>
    /// - Implementada como WaitAndRetryAsync para aplicar backoff exponencial.
    /// - Actualmente captura <see cref="TransientException"/> (puede ajustarse a otras excepciones Neo4j).
    /// - Parámetros configurables via <see cref="Neo4JOptions"/>:
    ///   * MaxRetryCount
    ///   * RetryDelayMilliseconds (base en ms; se aplica exponencial con Math.Pow)
    /// - El callback onRetry registra advertencias con el contador de reintentos y delay.
    ///
    /// Ejemplo conceptual de comportamiento:
    /// - retryAttempt = 1 -> delay = RetryDelayMilliseconds * 2^1
    /// - retryAttempt = 2 -> delay = RetryDelayMilliseconds * 2^2
    ///
    /// Consideraciones de testing:
    /// - Para tests unitarios, se recomienda exponer una sobrecarga del constructor que reciba una política inyectada
    ///   o usar alguna abstracción para sustituir <see cref="_retryPolicy"/>.
    /// - En integración, verificar que los reintentos ocurren simulando fallos transitorios y observando logs.
    /// </remarks>
    private readonly AsyncRetryPolicy _retryPolicy;

    private bool _disposed;

    /// <summary>
    /// Crea una instancia de <see cref="Neo4JDataAccess"/>.
    /// </summary>
    /// <param name="options">Configuración de Neo4j y parámetros de resiliencia.</param>
    /// <param name="logger">Logger para registrar eventos.</param>
    /// <exception cref="ArgumentNullException">Si options o logger son nulos.</exception>
    /// <remarks>
    /// Inicializa:
    /// - El <see cref="_driver"/> con los parámetros de conexión.
    /// - La <see cref="_retryPolicy"/> con WaitAndRetryAsync usando backoff exponencial.
    ///
    /// Cómo ajustar la política:
    /// - Cambie <see cref="Neo4JOptions.MaxRetryCount"/> y <see cref="Neo4JOptions.RetryDelayMilliseconds"/>.
    /// - Para modificaciones más avanzadas (CircuitBreaker, Fallback, Timeout) compose políticas de Polly.
    ///
    /// Ejemplo rápido (informativo, no código ejecutable aquí):
    /// - Policy.Handle<TransientException>().WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));
    /// </remarks>
    public Neo4JDataAccess(IOptions<Neo4JOptions> options, ILogger<Neo4JDataAccess> logger)
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

    /// <summary>
    /// Ejecuta una consulta de lectura contra Neo4j con política de reintento aplicada.
    /// </summary>
    /// <typeparam name="T">Tipo de resultado esperado.</typeparam>
    /// <param name="query">Func que recibe un <see cref="IAsyncQueryRunner"/> y ejecuta la consulta.</param>
    /// <param name="cancellationToken">Token de cancelación opcional.</param>
    /// <returns>Resultado de la consulta tipado como <typeparamref name="T"/>.</returns>
    /// <remarks>
    /// - La llamada a la sesión (session.ExecuteReadAsync) se envuelve dentro de <see cref="_retryPolicy"/>.
    /// - Si ocurre una excepción considerada transitoria, la política intentará reintentar según la configuración.
    /// - El <paramref name="cancellationToken"/> se pasa a <see cref="Policy{TResult}.ExecuteAsync(Func{CancellationToken, Task{TResult}}, CancellationToken)"/>
    ///   para permitir cancelar la operación completa (incluidos los reintentos).
    /// - Se registran métricas básicas de duración y errores.
    /// </remarks>
    public async Task<T> ExecuteReadAsync<T>(Func<IAsyncQueryRunner, Task<T>> query, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        await using var session = _driver.AsyncSession(o =>
        {
            if (!string.IsNullOrWhiteSpace(_options.Database))
            {
                o.WithDatabase(_options.Database);
            }
        });
        
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

    /// <summary>
    /// Ejecuta una consulta de escritura contra Neo4j con política de reintento aplicada.
    /// </summary>
    /// <typeparam name="T">Tipo de resultado esperado.</typeparam>
    /// <param name="query">Func que recibe un <see cref="IAsyncQueryRunner"/> para ejecutar la operación de escritura.</param>
    /// <param name="cancellationToken">Token de cancelación opcional.</param>
    /// <returns>Resultado de la operación tipado como <typeparamref name="T"/>.</returns>
    /// <remarks>
    /// - Similar a <see cref="ExecuteReadAsync{T}"/>, pero ejecuta operaciones de escritura.
    /// - Reintentos pueden provocar múltiples intentos de ejecución de la misma transacción; asegure idempotencia si es necesario.
    /// </remarks>
    public async Task<T> ExecuteWriteAsync<T>(Func<IAsyncQueryRunner, Task<T>> query, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        await using var session = _driver.AsyncSession(o =>
        {
            if (!string.IsNullOrWhiteSpace(_options.Database))
            {
                o.WithDatabase(_options.Database);
            }
        });
        
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
    /// <param name="cancellationToken">Token de cancelación opcional.</param>
    /// <returns>True si la conectividad es correcta; false en caso contrario.</returns>
    /// <remarks>
    /// - No usa la política de reintento ya que el driver expone VerifyConnectivityAsync directamente.
    /// - Se recomienda usar este método para endpoints de health checks y no para operaciones de negocio.
    /// </remarks>
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

    /// <summary>
    /// Lógica de liberación de recursos.
    /// </summary>
    /// <param name="disposing">True si se están liberando recursos gestionados.</param>
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