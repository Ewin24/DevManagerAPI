namespace Infrastructure.Data.Configuration.Projects;

using Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> entity)
    {
        entity.HasIndex(e => new { e.OrganizationId, e.Status }, "IX_Projects_Org_Status")
            .HasFilter("([IsDeleted]=(0))");

        entity.HasIndex(e => new { e.OrganizationId, e.Code }, "UX_Projects_Org_Code")
            .IsUnique()
            .HasFilter("([Code] IS NOT NULL AND [IsDeleted]=(0))");

        entity.Property(e => e.Id).ValueGeneratedNever();
        entity.Property(e => e.ComplexityLevel).HasDefaultValue((byte)1);
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

        entity.HasOne(d => d.Organization).WithMany(p => p.Projects)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Projects_Organizations");

        entity.HasOne(d => d.ProjectStatusNavigation).WithMany(p => p.Projects)
            .HasForeignKey(d => d.Status)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Projects_Status");

        entity.HasOne(d => d.ComplexityLevelNavigation).WithMany(p => p.Projects)
            .HasForeignKey(d => d.ComplexityLevel)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Projects_Complexity");
    }
}