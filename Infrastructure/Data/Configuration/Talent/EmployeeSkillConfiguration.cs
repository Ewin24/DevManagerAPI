namespace Infrastructure.Data.Configuration.Talent;

using Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class EmployeeSkillConfiguration : IEntityTypeConfiguration<EmployeeSkill>
{
    public void Configure(EntityTypeBuilder<EmployeeSkill> entity)
    {
        entity.HasIndex(e => new { e.OrganizationId, e.SkillId }, "IX_EmployeeSkills_Org_Skill")
            .HasFilter("([IsDeleted]=(0))");

        entity.HasIndex(e => new { e.OrganizationId, e.UserId, e.SkillId }, "UX_EmployeeSkills_Org_User_Skill")
            .IsUnique()
            .HasFilter("([IsDeleted]=(0))");

        entity.Property(e => e.Id).ValueGeneratedNever();
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

        entity.HasOne(d => d.Organization).WithMany(p => p.EmployeeSkills)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_EmployeeSkills_Organizations");

        entity.HasOne(d => d.Skill).WithMany(p => p.EmployeeSkills)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_EmployeeSkills_Skills");

        entity.HasOne(d => d.User).WithMany(p => p.EmployeeSkillUsers)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_EmployeeSkills_Users");

        entity.HasOne(d => d.ValidatedByUser).WithMany(p => p.EmployeeSkillValidatedByUsers)
            .HasConstraintName("FK_EmployeeSkills_Validator");

        entity.HasOne(d => d.SkillLevelNavigation).WithMany(p => p.EmployeeSkills)
            .HasForeignKey(d => d.Level)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_EmployeeSkills_Level");
    }
}