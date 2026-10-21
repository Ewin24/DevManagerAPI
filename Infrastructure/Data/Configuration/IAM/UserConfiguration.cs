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

        entity.HasIndex(e => new { e.OrganizationId, e.DocumentType, e.DocumentNumber }, "UX_Users_Org_Document")
            .IsUnique()
            .HasFilter("([DocumentNumber] IS NOT NULL AND [IsDeleted]=(0))");

        entity.Property(e => e.Id).ValueGeneratedNever();
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
        entity.Property(e => e.IsActive).HasDefaultValue(true);

        entity.HasOne(d => d.Organization).WithMany(p => p.Users)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Users_Organizations");
    }
}