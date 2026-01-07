namespace Infrastructure.Data.Configuration.Projects;

using Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ProjectParticipationConfiguration : IEntityTypeConfiguration<ProjectParticipation>
{
    public void Configure(EntityTypeBuilder<ProjectParticipation> entity)
    {
        entity.HasIndex(e => new { e.OrganizationId, e.ProjectId, e.UserId }, "UX_ProjectParticipation_Org_Project_User")
            .IsUnique();

        entity.Property(e => e.Id).ValueGeneratedNever();

        entity.HasOne(d => d.Organization).WithMany(p => p.ProjectParticipations)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_ProjectParticipation_Organizations");

        entity.HasOne(d => d.Project).WithMany(p => p.ProjectParticipations)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_ProjectParticipation_Projects");

        entity.HasOne(d => d.User).WithMany(p => p.ProjectParticipations)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_ProjectParticipation_Users");
    }
}