using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Entities;

[Table("Projects", Schema = "projects")]
public partial class Project
{
    [Key]
    public Guid Id { get; set; }

    public Guid OrganizationId { get; set; }

    [StringLength(40)]
    public string? Code { get; set; }

    [StringLength(160)]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public byte ComplexityLevel { get; set; }

    public byte Status { get; set; }

    [Precision(3)]
    public DateTime CreatedAt { get; set; }

    public Guid? CreatedByUserId { get; set; }

    [Precision(3)]
    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedByUserId { get; set; }

    public bool IsDeleted { get; set; }

    [ForeignKey("OrganizationId")]
    [InverseProperty("Projects")]
    public virtual Organization Organization { get; set; } = null!;

    [InverseProperty("Project")]
    public virtual ICollection<ProjectApplication> ProjectApplications { get; set; } = new List<ProjectApplication>();

    [InverseProperty("Project")]
    public virtual ICollection<ProjectAssignment> ProjectAssignments { get; set; } = new List<ProjectAssignment>();

    [InverseProperty("Project")]
    public virtual ICollection<ProjectParticipation> ProjectParticipations { get; set; } = new List<ProjectParticipation>();

    [InverseProperty("Project")]
    public virtual ICollection<ProjectRole> ProjectRoles { get; set; } = new List<ProjectRole>();

    [InverseProperty("Project")]
    public virtual ICollection<ProjectSkillRequirement> ProjectSkillRequirements { get; set; } = new List<ProjectSkillRequirement>();

    [InverseProperty("Project")]
    public virtual ICollection<SkillEvaluation> SkillEvaluations { get; set; } = new List<SkillEvaluation>();
}
