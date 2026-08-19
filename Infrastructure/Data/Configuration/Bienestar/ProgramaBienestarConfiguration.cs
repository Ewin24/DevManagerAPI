namespace Infrastructure.Data.Configuration.Bienestar;

using Domain.Entities.Bienestar;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ProgramaBienestarConfiguration : IEntityTypeConfiguration<ProgramaBienestar>
{
    public void Configure(EntityTypeBuilder<ProgramaBienestar> builder)
    {
        builder.ToTable("ProgramasBienestar", "bienestar");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).HasDefaultValueSql("NEWID()");
        builder.Property(e => e.Nombre).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Descripcion).HasMaxLength(500);
        builder.Property(e => e.Presupuesto).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(e => e.Activo).HasDefaultValue(true);
        builder.Property(e => e.CreatedAt).HasColumnType("datetime2(3)").HasDefaultValueSql("sysutcdatetime()");
        builder.Property(e => e.UpdatedAt).HasColumnType("datetime2(3)");
        builder.Property(e => e.DeletedAt).HasColumnType("datetime2(3)");

        builder.HasOne(e => e.Organization)
            .WithMany()
            .HasForeignKey(e => e.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
