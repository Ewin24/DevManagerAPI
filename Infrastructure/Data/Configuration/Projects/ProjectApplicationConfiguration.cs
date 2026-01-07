namespace Infrastructure.Data.Configuration.Projects;

using Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ProjectApplicationConfiguration : IEntityTypeConfiguration<ProjectApplication>
{
    public void Configure(EntityTypeBuilder<ProjectApplication> entity)
    {
        entity.HasIndex(e => new { e.OrganizationId, e.ProjectId, e.Status }, "IX_ProjectApplications_Org_Project_Status")
            .HasFilter("([IsDeleted]=(0))");

        entity.HasIndex(e => new { e.OrganizationId, e.ProjectId, e.UserId }, "UX_ProjectApplications_Org_Project_User")
            .IsUnique()
            .HasFilter("([IsDeleted]=(0))");

        entity.Property(e => e.Id).ValueGeneratedNever();
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

        entity.HasOne(d => d.Organization).WithMany(p => p.ProjectApplications)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_ProjectApplications_Organizations");

        entity.HasOne(d => d.Project).WithMany(p => p.ProjectApplications)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_ProjectApplications_Projects");

        entity.HasOne(d => d.ReviewedByUser).WithMany(p => p.ProjectApplicationReviewedByUsers)
            .HasForeignKey(d => d.ReviewedByUserId)
            .HasConstraintName("FK_ProjectApplications_Users_ReviewedBy");

        entity.HasOne(d => d.User).WithMany(p => p.ProjectApplicationUsers)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_ProjectApplications_Users");
    }
}