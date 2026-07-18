namespace Infrastructure.Data.Configuration.IAM;

using Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class PersonOrganizationConfiguration : IEntityTypeConfiguration<PersonOrganization>
{
    public void Configure(EntityTypeBuilder<PersonOrganization> entity)
    {
        entity.HasKey(e => new { e.PersonId, e.OrganizationId });

        entity.Property(e => e.MembershipType)
            .HasMaxLength(50)
            .IsRequired();

        entity.Property(e => e.JoinedAt)
            .HasDefaultValueSql("(sysutcdatetime())");

        entity.Property(e => e.Status)
            .HasConversion<int>()
            .HasDefaultValue(Domain.Enums.MembershipStatus.Active);

        entity.HasOne(e => e.Person)
            .WithMany(e => e.PersonOrganizations)
            .HasForeignKey(e => e.PersonId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_PersonOrganizations_Person");

        entity.HasOne(e => e.Organization)
            .WithMany(e => e.PersonOrganizations)
            .HasForeignKey(e => e.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_PersonOrganizations_Organization");
    }
}
