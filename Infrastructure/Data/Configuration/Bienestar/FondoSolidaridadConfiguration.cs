namespace Infrastructure.Data.Configuration.Bienestar;

using Domain.Entities.Bienestar;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class FondoSolidaridadConfiguration : IEntityTypeConfiguration<FondoSolidaridad>
{
    public void Configure(EntityTypeBuilder<FondoSolidaridad> builder)
    {
        builder.ToTable("FondoSolidaridad", "bienestar");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).HasDefaultValueSql("NEWID()");
        builder.Property(e => e.TotalExcedentes).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(e => e.AporteFondo).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(e => e.SaldoDisponible).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(e => e.TotalDesembolsado).HasColumnType("decimal(18,2)").HasDefaultValue(0);
        builder.Property(e => e.Vigente).HasDefaultValue(true);
        builder.Property(e => e.Observaciones).HasMaxLength(500);
        builder.Property(e => e.CreatedAt).HasColumnType("datetime2(3)").HasDefaultValueSql("sysutcdatetime()");
        builder.Property(e => e.UpdatedAt).HasColumnType("datetime2(3)");
        builder.Property(e => e.DeletedAt).HasColumnType("datetime2(3)");

        builder.HasIndex(e => new { e.OrganizationId, e.Periodo })
            .IsUnique()
            .HasDatabaseName("UX_FondoSolidaridad_Org_Periodo");
    }
}
