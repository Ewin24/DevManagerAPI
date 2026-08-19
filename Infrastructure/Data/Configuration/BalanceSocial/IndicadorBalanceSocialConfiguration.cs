namespace Infrastructure.Data.Configuration.BalanceSocial;

using Domain.Entities.BalanceSocial;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class IndicadorBalanceSocialConfiguration : IEntityTypeConfiguration<IndicadorBalanceSocial>
{
    public void Configure(EntityTypeBuilder<IndicadorBalanceSocial> builder)
    {
        builder.ToTable("IndicadoresBalanceSocial", "balance_social");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).HasDefaultValueSql("NEWID()");
        builder.Property(e => e.Anio).IsRequired();
        builder.Property(e => e.HorasEducacion).HasDefaultValue(0);
        builder.Property(e => e.ParticipacionAsambleas).HasDefaultValue(0);
        builder.Property(e => e.ParticipacionComites).HasDefaultValue(0);
        builder.Property(e => e.AportesSociales).HasColumnType("decimal(18,2)").HasDefaultValue(0);
        builder.Property(e => e.BeneficiosRecibidos).HasColumnType("decimal(18,2)").HasDefaultValue(0);
        builder.Property(e => e.CumpleEducacion).HasDefaultValue(false);
        builder.Property(e => e.IndiceBalanceSocial).HasColumnType("decimal(5,2)").HasDefaultValue(0);
        builder.Property(e => e.Observaciones).HasMaxLength(500);
        builder.Property(e => e.CreatedAt).HasColumnType("datetime2(3)").HasDefaultValueSql("sysutcdatetime()");
        builder.Property(e => e.UpdatedAt).HasColumnType("datetime2(3)");
        builder.Property(e => e.DeletedAt).HasColumnType("datetime2(3)");

        builder.HasIndex(e => new { e.AsociadoId, e.Anio })
            .IsUnique()
            .HasDatabaseName("UX_IndicadorBalance_Asociado_Anio");
    }
}
