namespace Infrastructure.Data.Configuration.Organos;

using Domain.Entities.Organos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class MiembroOrganoConfiguration : IEntityTypeConfiguration<MiembroOrgano>
{
    public void Configure(EntityTypeBuilder<MiembroOrgano> builder)
    {
        builder.ToTable("MiembrosOrgano");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Cargo)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.FechaInicio)
            .IsRequired();

        builder.HasOne(e => e.Organo)
            .WithMany(e => e.Miembros)
            .HasForeignKey(e => e.OrganoId);
    }
}
