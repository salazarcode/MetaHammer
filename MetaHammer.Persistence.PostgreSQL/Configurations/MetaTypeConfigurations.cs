/*
using MetaHammer.Persistence.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MetaHammer.Persistence.PostgreSQL.Configurations;

public class MetaTypeConfiguration : IEntityTypeConfiguration<MetaTypeDM>
{
    public void Configure(EntityTypeBuilder<MetaTypeDM> builder)
    {
        builder.ToTable("MetaTypes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired();

        // Configuración clave: Mapear string a columna jsonb
        builder.Property(x => x.MetaProperties)
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(x => x.MetaRelations)
            .HasColumnType("jsonb")
            .IsRequired();

        // Índices para búsquedas rápidas por Tenant y Nombre
        builder.HasIndex(x => new { x.TenantId, x.Name }).IsUnique();
    }
}
*/
