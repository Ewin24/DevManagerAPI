namespace Infrastructure.Data.Configuration.Reporting;

using Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class RecommendationRuleConfiguration : IEntityTypeConfiguration<RecommendationRule>
{
    public void Configure(EntityTypeBuilder<RecommendationRule> entity)
    {
        entity.HasIndex(e => new { e.OrganizationId, e.IsActive }, "IX_RecommendationRules_Org_IsActive");

        entity.Property(e => e.Id).ValueGeneratedNever();
        entity.Property(e => e.IsActive).HasDefaultValue(true);

        entity.HasOne(d => d.Organization).WithMany(p => p.RecommendationRules)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_RecommendationRules_Organizations");
    }
}
