namespace Infrastructure.Data.Configuration.Organos;

using Domain.Entities.Organos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class AsambleaConfiguration : IEntityTypeConfiguration<Asamblea>
{
    public void Configure(EntityTypeBuilder<Asamblea> builder)
    {
        builder.ToTable("Asambleas");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Tipo)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(e => e.Convocatoria)
            .IsRequired();

        builder.Property(e => e.QuorumMinimo)
            .IsRequired();

        builder.Property(e => e.Resultados)
            .HasMaxLength(2000);

        builder.HasOne(e => e.Organo)
            .WithMany(e => e.Asambleas)
            .HasForeignKey(e => e.OrganoId);

        builder.HasMany(e => e.Votos)
            .WithOne(e => e.Asamblea)
            .HasForeignKey(e => e.AsambleaId);

        builder.HasMany(e => e.Actas)
            .WithOne(e => e.Asamblea)
            .HasForeignKey(e => e.AsambleaId);
    }
}
