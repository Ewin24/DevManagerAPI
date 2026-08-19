namespace Infrastructure.Data.Configuration.SST;

using Domain.Entities.SST;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ExamenMedicoConfiguration : IEntityTypeConfiguration<ExamenMedico>
{
    public void Configure(EntityTypeBuilder<ExamenMedico> builder)
    {
        builder.ToTable("ExamenesMedicos", "sst");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).HasDefaultValueSql("NEWID()");
        builder.Property(e => e.TipoExamen).IsRequired().HasColumnType("tinyint");
        builder.Property(e => e.FechaProgramado).IsRequired().HasColumnType("datetime2(3)");
        builder.Property(e => e.FechaRealizado).HasColumnType("datetime2(3)");
        builder.Property(e => e.Resultado).HasMaxLength(100);
        builder.Property(e => e.ArchivoUrl).HasMaxLength(500);
        builder.Property(e => e.Observaciones).HasMaxLength(1000);
        builder.Property(e => e.CreatedAt).HasColumnType("datetime2(3)").HasDefaultValueSql("sysutcdatetime()");
        builder.Property(e => e.UpdatedAt).HasColumnType("datetime2(3)");
        builder.Property(e => e.DeletedAt).HasColumnType("datetime2(3)");

        builder.HasIndex(e => e.AsociadoId);
        builder.HasIndex(e => e.OrganizationId);
        builder.HasIndex(e => new { e.OrganizationId, e.FechaProgramado });
    }
}
