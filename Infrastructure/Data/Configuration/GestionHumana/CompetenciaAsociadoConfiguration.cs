namespace Infrastructure.Data.Configuration.GestionHumana;

using Domain.Entities.GestionHumana;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CompetenciaAsociadoConfiguration : IEntityTypeConfiguration<CompetenciaAsociado>
{
    public void Configure(EntityTypeBuilder<CompetenciaAsociado> builder)
    {
        builder.ToTable("CompetenciasAsociado", "gestion_humana");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).HasDefaultValueSql("NEWID()");
        builder.Property(e => e.Competencia).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Nivel).IsRequired();
        builder.Property(e => e.Disponible).HasDefaultValue(true);
        builder.Property(e => e.Observaciones).HasMaxLength(500);
        builder.Property(e => e.CreatedAt).HasColumnType("datetime2(3)").HasDefaultValueSql("sysutcdatetime()");
        builder.Property(e => e.UpdatedAt).HasColumnType("datetime2(3)");
        builder.Property(e => e.DeletedAt).HasColumnType("datetime2(3)");

        builder.HasOne(e => e.Asociado)
            .WithMany()
            .HasForeignKey(e => e.AsociadoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
