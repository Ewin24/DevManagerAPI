namespace Infrastructure.Data.Configuration.Projects;

using Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ProjectAssignmentConfiguration : IEntityTypeConfiguration<ProjectAssignment>
{
    public void Configure(EntityTypeBuilder<ProjectAssignment> entity)
    {
        entity.HasIndex(e => new { e.OrganizationId, e.ProjectId, e.Status }, "IX_ProjectAssignments_Org_Project_Status")
            .HasFilter("([IsDeleted]=(0))");

        entity.HasIndex(e => new { e.OrganizationId, e.ProjectId, e.UserId }, "UX_ProjectAssignments_Org_Project_User_Active")
            .IsUnique()
            .HasFilter("([IsDeleted]=(0) AND [Status]<>(3))");

        entity.Property(e => e.Id).ValueGeneratedNever();
        entity.Property(e => e.AssignedAt).HasDefaultValueSql("(sysutcdatetime())");

        entity.HasOne(d => d.AssignedByUser).WithMany(p => p.ProjectAssignmentAssignedByUsers)
            .HasForeignKey(d => d.AssignedByUserId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_ProjectAssignments_Users_AssignedBy");

        entity.HasOne(d => d.Organization).WithMany(p => p.ProjectAssignments)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_ProjectAssignments_Organizations");

        entity.HasOne(d => d.Project).WithMany(p => p.ProjectAssignments)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_ProjectAssignments_Projects");

        entity.HasOne(d => d.ProjectRole).WithMany(p => p.ProjectAssignments)
            .HasConstraintName("FK_ProjectAssignments_ProjectRoles");

        entity.HasOne(d => d.User).WithMany(p => p.ProjectAssignmentUsers)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_ProjectAssignments_Users");
    }
}