namespace Infrastructure.Data.Configuration.GestionHumana;

using Domain.Entities.GestionHumana;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ProgramaEducacionConfiguration : IEntityTypeConfiguration<ProgramaEducacion>
{
    public void Configure(EntityTypeBuilder<ProgramaEducacion> builder)
    {
        builder.ToTable("ProgramasEducacion", "gestion_humana");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).HasDefaultValueSql("NEWID()");
        builder.Property(e => e.Nombre).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Descripcion).HasMaxLength(500);
        builder.Property(e => e.Tipo).IsRequired();
        builder.Property(e => e.Horas).IsRequired();
        builder.Property(e => e.Activo).HasDefaultValue(true);
        builder.Property(e => e.CreatedAt).HasColumnType("datetime2(3)").HasDefaultValueSql("sysutcdatetime()");
        builder.Property(e => e.UpdatedAt).HasColumnType("datetime2(3)");
        builder.Property(e => e.DeletedAt).HasColumnType("datetime2(3)");
    }
}
