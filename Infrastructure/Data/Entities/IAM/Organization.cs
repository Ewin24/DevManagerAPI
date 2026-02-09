using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Data.Entities;

[Table("Organizations", Schema = "iam")]
[Index("Nit", Name = "UQ_Organizations_Nit", IsUnique = true)]
public partial class Organization
{
    [Key]
    public Guid Id { get; set; }

    [StringLength(160)]
    public string Name { get; set; } = null!;

    [StringLength(200)]
    public string? LegalName { get; set; }

    [StringLength(30)]
    public string? Nit { get; set; }

    public bool IsActive { get; set; }

    [Precision(3)]
    public DateTime CreatedAt { get; set; }

    public Guid? CreatedByUserId { get; set; }

    [Precision(3)]
    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedByUserId { get; set; }

    public bool IsDeleted { get; set; }

    [Precision(3)]
    public DateTime? DeletedAt { get; set; }

    public Guid? DeletedByUserId { get; set; }

    [InverseProperty("Organization")]
    public virtual ICollection<Certification> Certifications { get; set; } = new List<Certification>();

    [InverseProperty("Organization")]
    public virtual ICollection<EmployeeProfile> EmployeeProfiles { get; set; } = new List<EmployeeProfile>();

    [InverseProperty("Organization")]
    public virtual ICollection<EmployeeSkill> EmployeeSkills { get; set; } = new List<EmployeeSkill>();

    [InverseProperty("Organization")]
    public virtual ICollection<ProjectApplication> ProjectApplications { get; set; } = new List<ProjectApplication>();

    [InverseProperty("Organization")]
    public virtual ICollection<ProjectAssignment> ProjectAssignments { get; set; } = new List<ProjectAssignment>();

    [InverseProperty("Organization")]
    public virtual ICollection<ProjectParticipation> ProjectParticipations { get; set; } = new List<ProjectParticipation>();

    [InverseProperty("Organization")]
    public virtual ICollection<ProjectRole> ProjectRoles { get; set; } = new List<ProjectRole>();

    [InverseProperty("Organization")]
    public virtual ICollection<ProjectSkillRequirement> ProjectSkillRequirements { get; set; } = new List<ProjectSkillRequirement>();

    [InverseProperty("Organization")]
    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    [InverseProperty("Organization")]
    public virtual ICollection<RecommendationLog> RecommendationLogs { get; set; } = new List<RecommendationLog>();

    [InverseProperty("Organization")]
    public virtual ICollection<RecommendationRule> RecommendationRules { get; set; } = new List<RecommendationRule>();

    [InverseProperty("Organization")]
    public virtual ICollection<ReportSnapshot> ReportSnapshots { get; set; } = new List<ReportSnapshot>();

    [InverseProperty("Organization")]
    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();

    [InverseProperty("Organization")]
    public virtual ICollection<SkillEvaluation> SkillEvaluations { get; set; } = new List<SkillEvaluation>();

    [InverseProperty("Organization")]
    public virtual ICollection<Skill> Skills { get; set; } = new List<Skill>();

    [InverseProperty("Organization")]
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    [InverseProperty("Organization")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
