namespace Infrastructure.Data.Configuration.Talent;

using Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class EmployeeProfileConfiguration : IEntityTypeConfiguration<EmployeeProfile>
{
    public void Configure(EntityTypeBuilder<EmployeeProfile> entity)
    {
        entity.HasIndex(e => e.OrganizationId, "IX_EmployeeProfiles_Org")
            .HasFilter("([IsDeleted]=(0))");

        entity.Property(e => e.UserId).ValueGeneratedNever();
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

        entity.HasOne(d => d.Organization).WithMany(p => p.EmployeeProfiles)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_EmployeeProfiles_Organizations");

        entity.HasOne(d => d.User).WithOne(p => p.EmployeeProfile)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_EmployeeProfiles_Users");
    }
}
