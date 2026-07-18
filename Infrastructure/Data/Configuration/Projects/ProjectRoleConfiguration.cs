namespace Infrastructure.Data.Configuration.Projects;

using Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ProjectRoleConfiguration : IEntityTypeConfiguration<ProjectRole>
{
    public void Configure(EntityTypeBuilder<ProjectRole> entity)
    {
        entity.HasIndex(e => new { e.OrganizationId, e.ProjectId, e.Name }, "UX_ProjectRoles_Org_Project_Name")
            .IsUnique()
            .HasFilter("([IsDeleted]=(0))");

        entity.Property(e => e.Id).ValueGeneratedNever();
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
        entity.Property(e => e.NeededCount).HasDefaultValue((short)1);

        entity.HasOne(d => d.Organization).WithMany(p => p.ProjectRoles)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_ProjectRoles_Organizations");

        entity.HasOne(d => d.Project).WithMany(p => p.ProjectRoles)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_ProjectRoles_Projects");
    }
}