namespace Infrastructure.Data.Configuration.HabeasData;

using Domain.Entities.HabeasData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class SolicitudARCOConfiguration : IEntityTypeConfiguration<SolicitudARCO>
{
    public void Configure(EntityTypeBuilder<SolicitudARCO> builder)
    {
        builder.ToTable("SolicitudesARCO", "habeasdata");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).HasDefaultValueSql("NEWID()");
        builder.Property(e => e.Tipo).IsRequired().HasColumnType("tinyint");
        builder.Property(e => e.Fecha).IsRequired().HasColumnType("datetime2(3)");
        builder.Property(e => e.Estado).IsRequired().HasColumnType("tinyint").HasDefaultValueSql("1");
        builder.Property(e => e.Descripcion).IsRequired().HasMaxLength(2000);
        builder.Property(e => e.Respuesta).HasMaxLength(2000);
        builder.Property(e => e.FechaRespuesta).HasColumnType("datetime2(3)");
        builder.Property(e => e.Radicado).HasMaxLength(50);
        builder.Property(e => e.CreatedAt).HasColumnType("datetime2(3)").HasDefaultValueSql("sysutcdatetime()");
        builder.Property(e => e.UpdatedAt).HasColumnType("datetime2(3)");
        builder.Property(e => e.DeletedAt).HasColumnType("datetime2(3)");

        builder.HasIndex(e => e.AsociadoId);
        builder.HasIndex(e => e.OrganizationId);
        builder.HasIndex(e => e.Estado);
    }
}
