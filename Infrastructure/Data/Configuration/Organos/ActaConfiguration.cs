namespace Infrastructure.Data.Configuration.Organos;

using Domain.Entities.Organos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ActaConfiguration : IEntityTypeConfiguration<Acta>
{
    public void Configure(EntityTypeBuilder<Acta> builder)
    {
        builder.ToTable("Actas");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.TipoSesion)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.Decisiones)
            .IsRequired();

        builder.Property(e => e.ConvocatoriaUrl)
            .HasMaxLength(500);

        builder.Property(e => e.ActaUrl)
            .HasMaxLength(500);

        builder.Property(e => e.Observaciones)
            .HasMaxLength(1000);

        builder.HasOne(e => e.Organo)
            .WithMany(e => e.Actas)
            .HasForeignKey(e => e.OrganoId);

        builder.HasOne(e => e.Asamblea)
            .WithMany(e => e.Actas)
            .HasForeignKey(e => e.AsambleaId);
    }
}
