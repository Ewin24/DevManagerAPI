namespace Infrastructure.Data.Configuration.Nomina;

using Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class PilaAporteConfiguration : IEntityTypeConfiguration<PilaAporte>
{
    public void Configure(EntityTypeBuilder<PilaAporte> entity)
    {
        entity.Property(e => e.Id).ValueGeneratedNever();
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

        entity.Property(e => e.TipoAportante)
            .HasConversion<int>()
            .HasDefaultValue(Domain.Enums.PilaTipoAportante.Independiente);

        entity.Property(e => e.IngresoBase).HasPrecision(18, 2);
        entity.Property(e => e.AporteEPS).HasPrecision(18, 2);
        entity.Property(e => e.AportePension).HasPrecision(18, 2);
        entity.Property(e => e.AporteARL).HasPrecision(18, 2);
        entity.Property(e => e.Total).HasPrecision(18, 2);

        entity.HasIndex(e => new { e.AsociadoId, e.Periodo }, "IX_PilaAportes_Asociado_Periodo");
        entity.HasIndex(e => new { e.OrganizationId, e.Periodo }, "IX_PilaAportes_Org_Periodo");
    }
}
