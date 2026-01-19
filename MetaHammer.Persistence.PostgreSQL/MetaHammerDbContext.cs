/*
using MetaHammer.Persistence.PostgreSQL.Configurations;
using MetaHammer.Persistence.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;

namespace MetaHammer.Persistence.PostgreSQL;

public class MetaHammerDbContext: DbContext
{
    public MetaHammerDbContext(DbContextOptions<MetaHammerDbContext> options) : base(options)
    {
    }

    public DbSet<MetaTypeDM> MetaTypes { get; set; }
    public DbSet<MetaObjectDM> MetaObjects { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplicar las configuraciones separadas
        modelBuilder.ApplyConfiguration(new MetaTypeConfiguration());
        modelBuilder.ApplyConfiguration(new MetaObjectConfiguration());

        // Configuración global para PostgreSql (ej: convenciones de nombres snake_case si quisieras)
    }
}
*/
