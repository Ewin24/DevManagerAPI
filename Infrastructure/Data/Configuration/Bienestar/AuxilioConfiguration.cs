namespace Infrastructure.Data.Configuration.Bienestar;

using Domain.Entities.Bienestar;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class AuxilioConfiguration : IEntityTypeConfiguration<Auxilio>
{
    public void Configure(EntityTypeBuilder<Auxilio> builder)
    {
        builder.ToTable("Auxilios", "bienestar");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).HasDefaultValueSql("NEWID()");
        builder.Property(e => e.Tipo).IsRequired();
        builder.Property(e => e.Monto).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(e => e.Concepto).IsRequired().HasMaxLength(500);
        builder.Property(e => e.CreatedAt).HasColumnType("datetime2(3)").HasDefaultValueSql("sysutcdatetime()");
        builder.Property(e => e.UpdatedAt).HasColumnType("datetime2(3)");
        builder.Property(e => e.DeletedAt).HasColumnType("datetime2(3)");
        builder.Property(e => e.FechaEntrega).HasColumnType("datetime2(3)");
        builder.Property(e => e.FechaLimiteReintegro).HasColumnType("datetime2(3)");

        builder.HasOne(e => e.Asociado)
            .WithMany()
            .HasForeignKey(e => e.AsociadoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Solicitud)
            .WithMany()
            .HasForeignKey(e => e.SolicitudBienestarId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.Fondo)
            .WithMany()
            .HasForeignKey(e => e.FondoSolidaridadId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
