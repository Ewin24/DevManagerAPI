namespace Infrastructure.Data.Configuration.GestionHumana;

using Domain.Entities.GestionHumana;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class AsociadoEducacionConfiguration : IEntityTypeConfiguration<AsociadoEducacion>
{
    public void Configure(EntityTypeBuilder<AsociadoEducacion> builder)
    {
        builder.ToTable("AsociadosEducacion", "gestion_humana");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).HasDefaultValueSql("NEWID()");
        builder.Property(e => e.HorasCursadas).HasDefaultValue(0);
        builder.Property(e => e.Progreso).HasColumnType("decimal(5,2)").HasDefaultValue(0);
        builder.Property(e => e.Completado).HasDefaultValue(false);
        builder.Property(e => e.Resultado).HasMaxLength(500);
        builder.Property(e => e.CreatedAt).HasColumnType("datetime2(3)").HasDefaultValueSql("sysutcdatetime()");
        builder.Property(e => e.UpdatedAt).HasColumnType("datetime2(3)");
        builder.Property(e => e.DeletedAt).HasColumnType("datetime2(3)");

        builder.HasOne(e => e.Asociado)
            .WithMany()
            .HasForeignKey(e => e.AsociadoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Programa)
            .WithMany()
            .HasForeignKey(e => e.ProgramaEducacionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
