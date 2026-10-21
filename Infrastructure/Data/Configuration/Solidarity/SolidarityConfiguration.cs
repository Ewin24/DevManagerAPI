namespace Infrastructure.Data.Configuration.Solidarity;

using Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class AssociateConfiguration : IEntityTypeConfiguration<Associate>
{
    public void Configure(EntityTypeBuilder<Associate> entity)
    {
        entity.Property(e => e.UserId).ValueGeneratedNever();
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

        entity.HasOne(d => d.User).WithOne()
            .HasForeignKey<Associate>(d => d.UserId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Associates_Users");

        entity.HasOne(d => d.Organization).WithMany()
            .HasForeignKey(d => d.OrganizationId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Associates_Organizations");
    }
}

public class MembershipPeriodConfiguration : IEntityTypeConfiguration<MembershipPeriod>
{
    public void Configure(EntityTypeBuilder<MembershipPeriod> entity)
    {
        entity.HasIndex(e => new { e.OrganizationId, e.UserId }, "UX_MembershipPeriods_Org_User_Open")
            .IsUnique()
            .HasFilter("([WithdrawalDate] IS NULL AND [IsDeleted]=(0))");

        entity.Property(e => e.Id).ValueGeneratedNever();
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

        entity.HasOne(d => d.Associate).WithMany(p => p.MembershipPeriods)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_MembershipPeriods_Associates");
    }
}

public class DataPolicyVersionConfiguration : IEntityTypeConfiguration<DataPolicyVersion>
{
    public void Configure(EntityTypeBuilder<DataPolicyVersion> entity)
    {
        entity.HasIndex(e => new { e.OrganizationId, e.Version }, "UX_DataPolicyVersions_Org_Version")
            .IsUnique()
            .HasFilter("([IsDeleted]=(0))");

        entity.Property(e => e.Id).ValueGeneratedNever();
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
    }
}

public class DataConsentConfiguration : IEntityTypeConfiguration<DataConsent>
{
    public void Configure(EntityTypeBuilder<DataConsent> entity)
    {
        entity.HasIndex(e => new { e.OrganizationId, e.UserId, e.PolicyVersionId }, "UX_DataConsents_Org_User_Policy_Active")
            .IsUnique()
            .HasFilter("([RevokedAt] IS NULL)");

        entity.Property(e => e.Id).ValueGeneratedNever();
        entity.Property(e => e.GrantedAt).HasDefaultValueSql("(sysutcdatetime())");
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

        entity.HasOne(d => d.PolicyVersion).WithMany()
            .HasForeignKey(d => d.PolicyVersionId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_DataConsents_PolicyVersions");
    }
}
