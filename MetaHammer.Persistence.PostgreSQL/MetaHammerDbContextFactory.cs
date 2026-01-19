/*
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MetaHammer.Persistence.PostgreSQL;

public class MetaHammerDbContextFactory : IDesignTimeDbContextFactory<MetaHammerDbContext>
{
    public MetaHammerDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MetaHammerDbContext>();

        // Cadena de conexión para TIEMPO DE DISEÑO (Creación de migraciones).
        // No te preocupes, en producción se usará la que inyectes en tu API.
        // Asegúrate de que apunte a una DB de desarrollo local.
        var connectionString = "Host=localhost;Database=metahammer;Username=postgres;Password='Las lanzas coloradas.'";

        optionsBuilder.UseNpgsql(connectionString);

        return new MetaHammerDbContext(optionsBuilder.Options);
    }
}
*/
