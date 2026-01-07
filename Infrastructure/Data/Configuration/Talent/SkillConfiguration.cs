namespace Infrastructure.Data.Configuration.Talent;

using Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class SkillConfiguration : IEntityTypeConfiguration<Skill>
{
    public void Configure(EntityTypeBuilder<Skill> entity)
    {
        entity.HasIndex(e => new { e.OrganizationId, e.Name }, "UX_Skills_Org_Name")
            .IsUnique()
            .HasFilter("([IsDeleted]=(0))");

        entity.Property(e => e.Id).ValueGeneratedNever();
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

        entity.HasOne(d => d.Organization).WithMany(p => p.Skills)
            .HasConstraintName("FK_Skills_Organizations");
    }
}
