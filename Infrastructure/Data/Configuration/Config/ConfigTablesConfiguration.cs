namespace Infrastructure.Data.Configuration.Config;

using Infrastructure.Data.Entities.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ProjectStatusConfigConfiguration : IEntityTypeConfiguration<ProjectStatusConfig>
{
    public void Configure(EntityTypeBuilder<ProjectStatusConfig> entity)
    {
        entity.HasIndex(e => e.Code, "UQ_ProjectStatuses_Code").IsUnique();

        entity.Property(e => e.Id).ValueGeneratedNever();
        entity.Property(e => e.IsActive).HasDefaultValue(true);
        entity.Property(e => e.DisplayOrder).HasDefaultValue((byte)0);
        entity.Property(e => e.AllowsApplications).HasDefaultValue(false);
    }
}

public class ProjectComplexityLevelConfigConfiguration : IEntityTypeConfiguration<ProjectComplexityLevelConfig>
{
    public void Configure(EntityTypeBuilder<ProjectComplexityLevelConfig> entity)
    {
        entity.HasIndex(e => e.Code, "UQ_ProjectComplexityLevels_Code").IsUnique();

        entity.Property(e => e.Id).ValueGeneratedNever();
        entity.Property(e => e.ExperienceMultiplier).HasDefaultValue(1.0m);
        entity.Property(e => e.IsActive).HasDefaultValue(true);
        entity.Property(e => e.DisplayOrder).HasDefaultValue((byte)0);
    }
}

public class ApplicationStatusConfigConfiguration : IEntityTypeConfiguration<ApplicationStatusConfig>
{
    public void Configure(EntityTypeBuilder<ApplicationStatusConfig> entity)
    {
        entity.HasIndex(e => e.Code, "UQ_ApplicationStatuses_Code").IsUnique();

        entity.Property(e => e.Id).ValueGeneratedNever();
        entity.Property(e => e.IsActive).HasDefaultValue(true);
        entity.Property(e => e.DisplayOrder).HasDefaultValue((byte)0);
        entity.Property(e => e.RequiresReviewNotes).HasDefaultValue(false);
        entity.Property(e => e.IsFinalState).HasDefaultValue(false);
    }
}

public class AssignmentStatusConfigConfiguration : IEntityTypeConfiguration<AssignmentStatusConfig>
{
    public void Configure(EntityTypeBuilder<AssignmentStatusConfig> entity)
    {
        entity.HasIndex(e => e.Code, "UQ_AssignmentStatuses_Code").IsUnique();

        entity.Property(e => e.Id).ValueGeneratedNever();
        entity.Property(e => e.IsActive).HasDefaultValue(true);
        entity.Property(e => e.DisplayOrder).HasDefaultValue((byte)0);
        entity.Property(e => e.IsFinalState).HasDefaultValue(false);
    }
}

public class SkillLevelConfigConfiguration : IEntityTypeConfiguration<SkillLevelConfig>
{
    public void Configure(EntityTypeBuilder<SkillLevelConfig> entity)
    {
        entity.HasIndex(e => e.Code, "UQ_SkillLevels_Code").IsUnique();

        entity.Property(e => e.Id).ValueGeneratedNever();
        entity.Property(e => e.IsActive).HasDefaultValue(true);
        entity.Property(e => e.DisplayOrder).HasDefaultValue((byte)0);
    }
}

public class ContributionScoreConfigConfiguration : IEntityTypeConfiguration<ContributionScoreConfig>
{
    public void Configure(EntityTypeBuilder<ContributionScoreConfig> entity)
    {
        entity.HasIndex(e => e.Code, "UQ_ContributionScores_Code").IsUnique();

        entity.Property(e => e.Id).ValueGeneratedNever();
        entity.Property(e => e.ExperienceBonus).HasDefaultValue(0.0m);
        entity.Property(e => e.IsActive).HasDefaultValue(true);
        entity.Property(e => e.DisplayOrder).HasDefaultValue((byte)0);
    }
}

public class EvaluationSourceConfigConfiguration : IEntityTypeConfiguration<EvaluationSourceConfig>
{
    public void Configure(EntityTypeBuilder<EvaluationSourceConfig> entity)
    {
        entity.HasIndex(e => e.Code, "UQ_EvaluationSources_Code").IsUnique();

        entity.Property(e => e.Id).ValueGeneratedNever();
        entity.Property(e => e.IsActive).HasDefaultValue(true);
        entity.Property(e => e.DisplayOrder).HasDefaultValue((byte)0);
        entity.Property(e => e.IsAutomated).HasDefaultValue(false);
    }
}

public class SkillTypeConfigConfiguration : IEntityTypeConfiguration<SkillTypeConfig>
{
    public void Configure(EntityTypeBuilder<SkillTypeConfig> entity)
    {
        entity.HasIndex(e => e.Code, "UQ_SkillTypes_Code").IsUnique();

        entity.Property(e => e.IsActive).HasDefaultValue(true);
        entity.Property(e => e.DisplayOrder).HasDefaultValue((byte)0);
    }
}

public class SkillCategoryConfigConfiguration : IEntityTypeConfiguration<SkillCategoryConfig>
{
    public void Configure(EntityTypeBuilder<SkillCategoryConfig> entity)
    {
        entity.HasIndex(e => e.Code, "UQ_SkillCategories_Code").IsUnique();

        entity.Property(e => e.IsActive).HasDefaultValue(true);
        entity.Property(e => e.DisplayOrder).HasDefaultValue((byte)0);

        entity.HasOne(d => d.ParentCategory).WithMany(p => p.SubCategories)
            .HasForeignKey(d => d.ParentCategoryId)
            .HasConstraintName("FK_SkillCategories_Parent");
    }
}

public class AgentActionTypeConfigConfiguration : IEntityTypeConfiguration<AgentActionTypeConfig>
{
    public void Configure(EntityTypeBuilder<AgentActionTypeConfig> entity)
    {
        entity.HasIndex(e => e.Code, "UQ_AgentActionTypes_Code").IsUnique();

        entity.Property(e => e.IsActive).HasDefaultValue(true);
        entity.Property(e => e.DisplayOrder).HasDefaultValue((byte)0);
        entity.Property(e => e.RequiresApproval).HasDefaultValue(true);
    }
}

public class AgentActionStatusConfigConfiguration : IEntityTypeConfiguration<AgentActionStatusConfig>
{
    public void Configure(EntityTypeBuilder<AgentActionStatusConfig> entity)
    {
        entity.HasIndex(e => e.Code, "UQ_AgentActionStatuses_Code").IsUnique();

        entity.Property(e => e.IsActive).HasDefaultValue(true);
        entity.Property(e => e.DisplayOrder).HasDefaultValue((byte)0);
        entity.Property(e => e.IsFinalState).HasDefaultValue(false);
    }
}

public class SeniorityLevelConfigConfiguration : IEntityTypeConfiguration<SeniorityLevelConfig>
{
    public void Configure(EntityTypeBuilder<SeniorityLevelConfig> entity)
    {
        entity.HasIndex(e => e.Code, "UQ_SeniorityLevels_Code").IsUnique();

        entity.Property(e => e.IsActive).HasDefaultValue(true);
        entity.Property(e => e.DisplayOrder).HasDefaultValue((byte)0);
        entity.Property(e => e.MinYearsExperience).HasDefaultValue((byte)0);
    }
}