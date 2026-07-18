namespace Infrastructure.Data.Configuration.IAM;

using Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> entity)
    {
        entity.HasIndex(e => new { e.OrganizationId, e.Email }, "UX_Users_Org_Email")
            .IsUnique()
            .HasFilter("([IsDeleted]=(0))");

        entity.Property(e => e.Id).ValueGeneratedNever();
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
        entity.Property(e => e.IsActive).HasDefaultValue(true);

        entity.Property(e => e.PersonType)
            .HasConversion<int>()
            .HasDefaultValue(Domain.Enums.PersonType.Empleado);

        entity.Property(e => e.DocumentType)
            .HasMaxLength(5)
            .IsRequired(false);

        entity.Property(e => e.DocumentNumber)
            .HasMaxLength(20)
            .IsRequired(false);

        entity.HasOne(d => d.Organization).WithMany(p => p.Users)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Users_Organizations");
    }
}