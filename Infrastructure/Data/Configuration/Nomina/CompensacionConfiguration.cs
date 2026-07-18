namespace Infrastructure.Data.Configuration.Nomina;

using Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CompensacionConfiguration : IEntityTypeConfiguration<Compensacion>
{
    public void Configure(EntityTypeBuilder<Compensacion> entity)
    {
        entity.Property(e => e.Id).ValueGeneratedNever();
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

        entity.Property(e => e.Modelo)
            .HasConversion<int>()
            .HasDefaultValue(Domain.Enums.CompensacionModelo.DiasPorTarifa);

        entity.Property(e => e.ValorBase).HasPrecision(18, 2);
        entity.Property(e => e.ValorCalculado).HasPrecision(18, 2);

        entity.HasIndex(e => new { e.AsociadoId, e.Periodo }, "IX_Compensaciones_Asociado_Periodo");
    }
}
