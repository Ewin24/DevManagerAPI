namespace Infrastructure.Data.Configuration.Projects;

using Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ProjectSkillRequirementConfiguration : IEntityTypeConfiguration<ProjectSkillRequirement>
{
    public void Configure(EntityTypeBuilder<ProjectSkillRequirement> entity)
    {
        entity.HasIndex(e => new { e.OrganizationId, e.SkillId }, "IX_ProjectSkillRequirements_Org_Skill")
            .HasFilter("([IsDeleted]=(0))");

        entity.HasIndex(e => new { e.OrganizationId, e.ProjectId, e.SkillId }, "UX_ProjectSkillRequirements_Org_Project_Skill")
            .IsUnique()
            .HasFilter("([IsDeleted]=(0))");

        entity.Property(e => e.Id).ValueGeneratedNever();
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
        entity.Property(e => e.IsMandatory).HasDefaultValue(true);

        entity.HasOne(d => d.Organization).WithMany(p => p.ProjectSkillRequirements)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_ProjectSkillRequirements_Organizations");

        entity.HasOne(d => d.Project).WithMany(p => p.ProjectSkillRequirements)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_ProjectSkillRequirements_Projects");

        entity.HasOne(d => d.Skill).WithMany(p => p.ProjectSkillRequirements)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_ProjectSkillRequirements_Skills");

        entity.HasOne(d => d.RequiredLevelNavigation).WithMany(p => p.ProjectSkillRequirements)
            .HasForeignKey(d => d.RequiredLevel)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_ProjectSkillRequirements_Level");
    }
}