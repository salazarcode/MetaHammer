using Neo4j.Driver;

namespace Infrastructure.Repository.Neo4j.Interfaces;

public interface INeo4jDataAccess : IDisposable
{
    Task<T> ExecuteReadAsync<T>(Func<IAsyncQueryRunner, Task<T>> query, CancellationToken cancellationToken = default);
    Task<T> ExecuteWriteAsync<T>(Func<IAsyncQueryRunner, Task<T>> query, CancellationToken cancellationToken = default);
    Task ExecuteWriteAsync(Func<IAsyncQueryRunner, Task> query, CancellationToken cancellationToken = default);
    Task<bool> VerifyConnectivityAsync(CancellationToken cancellationToken = default);
}


