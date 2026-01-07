namespace Infrastructure.Data.Configuration.Talent;

using Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class SkillEvaluationConfiguration : IEntityTypeConfiguration<SkillEvaluation>
{
    public void Configure(EntityTypeBuilder<SkillEvaluation> entity)
    {
        entity.Property(e => e.Id).ValueGeneratedNever();
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

        entity.HasOne(d => d.Organization).WithMany(p => p.SkillEvaluations)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_SkillEvaluations_Organizations");

        entity.HasOne(d => d.Project).WithMany(p => p.SkillEvaluations)
            .HasConstraintName("FK_SkillEvaluations_Projects");

        entity.HasOne(d => d.Skill).WithMany(p => p.SkillEvaluations)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_SkillEvaluations_Skills");

        entity.HasOne(d => d.User).WithMany(p => p.SkillEvaluations)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_SkillEvaluations_Users");
    }
}
