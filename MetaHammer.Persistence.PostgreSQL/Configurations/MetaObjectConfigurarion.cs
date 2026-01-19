/*
using MetaHammer.Persistence.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MetaHammer.Persistence.PostgreSQL.Configurations;

public class MetaObjectConfiguration : IEntityTypeConfiguration<MetaObjectDM>
{
    public void Configure(EntityTypeBuilder<MetaObjectDM> builder)
    {
        builder.ToTable("MetaObjects");

        builder.HasKey(x => x.Id);

        // Relación fuerte SQL entre la Instancia y su Definición
        builder.HasOne(x => x.MetaType)
            .WithMany()
            .HasForeignKey(x => x.MetaTypeId)
            .OnDelete(DeleteBehavior.Restrict); // Evitar borrar un Tipo si tiene objetos

        // Los datos reales van en jsonb
        builder.Property(x => x.Properties)
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(x => x.Relations)
            .HasColumnType("jsonb")
            .IsRequired();

        // Índice GIN: Vital para poder buscar DENTRO del JSON
        // Ej: Buscar todos los objetos donde PropertiesData->'Age' > 20
        builder.HasIndex(x => x.Properties).HasMethod("gin");
        builder.HasIndex(x => x.Relations).HasMethod("gin");

        builder.HasIndex(x => x.TenantId);
    }
}
*/
