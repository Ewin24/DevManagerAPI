namespace Infrastructure.Data.Configuration.Bienestar;

using Domain.Entities.Bienestar;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class SolicitudBienestarConfiguration : IEntityTypeConfiguration<SolicitudBienestar>
{
    public void Configure(EntityTypeBuilder<SolicitudBienestar> builder)
    {
        builder.ToTable("SolicitudesBienestar", "bienestar");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).HasDefaultValueSql("NEWID()");
        builder.Property(e => e.TipoAuxilio).IsRequired();
        builder.Property(e => e.MontoSolicitado).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(e => e.MontoAprobado).HasColumnType("decimal(18,2)");
        builder.Property(e => e.Estado).IsRequired().HasDefaultValue(Domain.Enums.EstadoSolicitudBienestar.Pendiente);
        builder.Property(e => e.Motivo).IsRequired().HasMaxLength(1000);
        builder.Property(e => e.ObservacionesResolucion).HasMaxLength(1000);
        builder.Property(e => e.CreatedAt).HasColumnType("datetime2(3)").HasDefaultValueSql("sysutcdatetime()");
        builder.Property(e => e.UpdatedAt).HasColumnType("datetime2(3)");
        builder.Property(e => e.DeletedAt).HasColumnType("datetime2(3)");
        builder.Property(e => e.FechaRequerida).HasColumnType("datetime2(3)");
        builder.Property(e => e.FechaResolucion).HasColumnType("datetime2(3)");

        builder.HasOne(e => e.Asociado)
            .WithMany()
            .HasForeignKey(e => e.AsociadoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Programa)
            .WithMany(p => p.Solicitudes)
            .HasForeignKey(e => e.ProgramaBienestarId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
