namespace Infrastructure.Data.Configuration.Reporting;

using Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class RecommendationLogConfiguration : IEntityTypeConfiguration<RecommendationLog>
{
    public void Configure(EntityTypeBuilder<RecommendationLog> entity)
    {
        entity.Property(e => e.Id).ValueGeneratedNever();
        entity.Property(e => e.GeneratedAt).HasDefaultValueSql("(sysutcdatetime())");

        entity.HasOne(d => d.GeneratedByUser).WithMany(p => p.RecommendationLogs)
            .HasForeignKey(d => d.GeneratedByUserId)
            .HasConstraintName("FK_RecommendationLogs_Users");

        entity.HasOne(d => d.Organization).WithMany(p => p.RecommendationLogs)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_RecommendationLogs_Organizations");
    }
}
