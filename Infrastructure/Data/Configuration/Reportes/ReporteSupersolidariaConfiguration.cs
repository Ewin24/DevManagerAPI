namespace Infrastructure.Data.Configuration.Reportes;

using Domain.Entities.Reportes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ReporteSupersolidariaConfiguration : IEntityTypeConfiguration<ReporteSupersolidaria>
{
    public void Configure(EntityTypeBuilder<ReporteSupersolidaria> builder)
    {
        builder.ToTable("ReportesSupersolidaria", "reportes");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).HasDefaultValueSql("NEWID()");
        builder.Property(e => e.Periodo).IsRequired().HasColumnType("datetime2(3)");
        builder.Property(e => e.BalanceSocialJson).HasColumnType("nvarchar(max)");
        builder.Property(e => e.AsociadosJson).HasColumnType("nvarchar(max)");
        builder.Property(e => e.CumplimientoJson).HasColumnType("nvarchar(max)");
        builder.Property(e => e.TipoReporte).IsRequired().HasMaxLength(50).HasDefaultValue("Trimestral");
        builder.Property(e => e.Enviado).HasDefaultValue(false);
        builder.Property(e => e.FechaEnvio).HasColumnType("datetime2(3)");
        builder.Property(e => e.Observaciones).HasMaxLength(1000);
        builder.Property(e => e.CreatedAt).HasColumnType("datetime2(3)").HasDefaultValueSql("sysutcdatetime()");
        builder.Property(e => e.UpdatedAt).HasColumnType("datetime2(3)");
        builder.Property(e => e.DeletedAt).HasColumnType("datetime2(3)");

        builder.HasIndex(e => e.OrganizationId);
        builder.HasIndex(e => new { e.OrganizationId, e.Periodo }).IsUnique();
    }
}
