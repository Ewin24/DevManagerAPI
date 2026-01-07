namespace Infrastructure.Data.Configuration.Talent;

using Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CertificationConfiguration : IEntityTypeConfiguration<Certification>
{
    public void Configure(EntityTypeBuilder<Certification> entity)
    {
        entity.HasIndex(e => new { e.OrganizationId, e.UserId }, "IX_Certifications_Org_User")
            .HasFilter("([IsDeleted]=(0))");

        entity.Property(e => e.Id).ValueGeneratedNever();
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

        entity.HasOne(d => d.Organization).WithMany(p => p.Certifications)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Certifications_Organizations");

        entity.HasOne(d => d.User).WithMany(p => p.Certifications)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Certifications_Users");
    }
}
