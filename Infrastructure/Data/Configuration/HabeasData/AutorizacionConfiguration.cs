namespace Infrastructure.Data.Configuration.HabeasData;

using Domain.Entities.HabeasData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class AutorizacionConfiguration : IEntityTypeConfiguration<Autorizacion>
{
    public void Configure(EntityTypeBuilder<Autorizacion> builder)
    {
        builder.ToTable("AutorizacionesHabeasData", "habeasdata");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).HasDefaultValueSql("NEWID()");
        builder.Property(e => e.FechaAutorizacion).IsRequired().HasColumnType("datetime2(3)");
        builder.Property(e => e.Vigencia).HasColumnType("datetime2(3)");
        builder.Property(e => e.Revocada).HasDefaultValue(false);
        builder.Property(e => e.FechaRevocacion).HasColumnType("datetime2(3)");
        builder.Property(e => e.Finalidad).IsRequired().HasMaxLength(500);
        builder.Property(e => e.MedioAutorizacion).IsRequired().HasMaxLength(50).HasDefaultValue("Digital");
        builder.Property(e => e.DireccionIp).HasMaxLength(50);
        builder.Property(e => e.CreatedAt).HasColumnType("datetime2(3)").HasDefaultValueSql("sysutcdatetime()");
        builder.Property(e => e.UpdatedAt).HasColumnType("datetime2(3)");
        builder.Property(e => e.DeletedAt).HasColumnType("datetime2(3)");

        builder.HasIndex(e => e.AsociadoId);
        builder.HasIndex(e => e.Revocada);
    }
}
