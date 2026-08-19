namespace Infrastructure.Data.Configuration.Organos;

using Domain.Entities.Organos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class OrganoConfiguration : IEntityTypeConfiguration<Organo>
{
    public void Configure(EntityTypeBuilder<Organo> builder)
    {
        builder.ToTable("Organos");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Nombre)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.Tipo)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(e => e.Descripcion)
            .HasMaxLength(500);

        builder.Property(e => e.FechaConstitucion)
            .IsRequired();

        builder.HasMany(e => e.Miembros)
            .WithOne(e => e.Organo)
            .HasForeignKey(e => e.OrganoId);

        builder.HasMany(e => e.Actas)
            .WithOne(e => e.Organo)
            .HasForeignKey(e => e.OrganoId);

        builder.HasMany(e => e.Asambleas)
            .WithOne(e => e.Organo)
            .HasForeignKey(e => e.OrganoId);
    }
}
