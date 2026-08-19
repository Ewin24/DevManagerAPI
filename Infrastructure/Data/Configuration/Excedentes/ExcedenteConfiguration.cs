namespace Infrastructure.Data.Configuration.Excedentes;

using Domain.Entities.Excedentes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ExcedenteConfiguration : IEntityTypeConfiguration<Excedente>
{
    public void Configure(EntityTypeBuilder<Excedente> builder)
    {
        builder.ToTable("Excedentes", "excedentes");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).HasDefaultValueSql("NEWID()");
        builder.Property(e => e.Periodo).IsRequired().HasColumnType("datetime2(3)");
        builder.Property(e => e.TotalExcedentes).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(e => e.ReservaProteccionAportes).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(e => e.FondoEducacion).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(e => e.FondoSolidaridad).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(e => e.Revalorizacion).HasColumnType("decimal(18,2)");
        builder.Property(e => e.RetornoCooperativo).HasColumnType("decimal(18,2)");
        builder.Property(e => e.AprobadoPorAsamblea).HasDefaultValue(false);
        builder.Property(e => e.Observaciones).HasMaxLength(1000);
        builder.Property(e => e.CreatedAt).HasColumnType("datetime2(3)").HasDefaultValueSql("sysutcdatetime()");
        builder.Property(e => e.UpdatedAt).HasColumnType("datetime2(3)");
        builder.Property(e => e.DeletedAt).HasColumnType("datetime2(3)");

        builder.HasIndex(e => e.OrganizationId);
        builder.HasIndex(e => new { e.OrganizationId, e.Periodo }).IsUnique();
    }
}
