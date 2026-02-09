using Neo4j.Driver;

namespace MetaHammer.Persistence.Neo4j;

public class Neo4jContext : IAsyncDisposable
{
    private readonly IDriver _driver;

    public Neo4jContext(string uri, string user, string password)
    {
        _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
    }

    public IAsyncSession CreateSession() => _driver.AsyncSession();

    public async ValueTask DisposeAsync()
    {
        await _driver.DisposeAsync();
    }
}
