namespace Infrastructure.Data.Configuration.SST;

using Domain.Entities.SST;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class AccidenteConfiguration : IEntityTypeConfiguration<Accidente>
{
    public void Configure(EntityTypeBuilder<Accidente> builder)
    {
        builder.ToTable("Accidentes", "sst");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).HasDefaultValueSql("NEWID()");
        builder.Property(e => e.Fecha).IsRequired().HasColumnType("datetime2(3)");
        builder.Property(e => e.Tipo).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Gravedad).IsRequired().HasColumnType("tinyint");
        builder.Property(e => e.ARL).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Descripcion).IsRequired().HasMaxLength(2000);
        builder.Property(e => e.FURAT).HasMaxLength(50);
        builder.Property(e => e.FechaInvestigacion).HasColumnType("datetime2(3)");
        builder.Property(e => e.InvestigacionCompletada).HasDefaultValue(false);
        builder.Property(e => e.Conclusiones).HasMaxLength(2000);
        builder.Property(e => e.Causas).HasMaxLength(2000);
        builder.Property(e => e.MedidasCorrectivas).HasMaxLength(2000);
        builder.Property(e => e.CreatedAt).HasColumnType("datetime2(3)").HasDefaultValueSql("sysutcdatetime()");
        builder.Property(e => e.UpdatedAt).HasColumnType("datetime2(3)");
        builder.Property(e => e.DeletedAt).HasColumnType("datetime2(3)");

        builder.HasIndex(e => e.AsociadoId);
        builder.HasIndex(e => e.OrganizationId);
        builder.HasIndex(e => e.InvestigacionCompletada);
    }
}
