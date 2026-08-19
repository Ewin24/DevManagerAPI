namespace Infrastructure.Data.Configuration.SST;

using Domain.Entities.SST;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class RiesgoConfiguration : IEntityTypeConfiguration<Riesgo>
{
    public void Configure(EntityTypeBuilder<Riesgo> builder)
    {
        builder.ToTable("Riesgos", "sst");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).HasDefaultValueSql("NEWID()");
        builder.Property(e => e.NivelRiesgo).IsRequired();
        builder.Property(e => e.Factor).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Descripcion).IsRequired().HasMaxLength(500);
        builder.Property(e => e.Activo).HasDefaultValue(true);
        builder.Property(e => e.Controles).HasMaxLength(1000);
        builder.Property(e => e.CreatedAt).HasColumnType("datetime2(3)").HasDefaultValueSql("sysutcdatetime()");
        builder.Property(e => e.UpdatedAt).HasColumnType("datetime2(3)");
        builder.Property(e => e.DeletedAt).HasColumnType("datetime2(3)");

        builder.HasIndex(e => e.OrganizationId);
        builder.HasIndex(e => e.Activo);
    }
}
