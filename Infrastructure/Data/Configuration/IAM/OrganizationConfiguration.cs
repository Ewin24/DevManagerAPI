namespace Infrastructure.Data.Configuration.IAM;

using Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> entity)
    {
        entity.Property(e => e.Id).ValueGeneratedNever();
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
        entity.Property(e => e.IsActive).HasDefaultValue(true);

        entity.Property(e => e.TipoOrganizacionSolidaria)
            .HasConversion<int>()
            .IsRequired(false);

        entity.Property(e => e.ParentId).IsRequired(false);

        entity.HasOne(e => e.Parent)
            .WithMany(e => e.Children)
            .HasForeignKey(e => e.ParentId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Organizations_Parent");
    }
}