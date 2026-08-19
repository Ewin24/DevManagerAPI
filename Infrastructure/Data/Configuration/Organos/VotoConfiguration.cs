namespace Infrastructure.Data.Configuration.Organos;

using Domain.Entities.Organos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class VotoConfiguration : IEntityTypeConfiguration<Voto>
{
    public void Configure(EntityTypeBuilder<Voto> builder)
    {
        builder.ToTable("Votos");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.VotoEmitido)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(e => e.Fecha)
            .IsRequired();

        builder.Property(e => e.Observaciones)
            .HasMaxLength(500);

        builder.HasOne(e => e.Asamblea)
            .WithMany(e => e.Votos)
            .HasForeignKey(e => e.AsambleaId);
    }
}
