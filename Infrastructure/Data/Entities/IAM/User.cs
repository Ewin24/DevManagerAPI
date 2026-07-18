using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Data.Entities;

[Table("Users", Schema = "iam")]
[Index("OrganizationId", "IsActive", Name = "IX_Users_Org_IsActive")]
public partial class User
{
    [Key]
    public Guid Id { get; set; }

    public Guid OrganizationId { get; set; }

    [StringLength(80)]
    public string FirstName { get; set; } = null!;

    [StringLength(80)]
    public string LastName { get; set; } = null!;

    [StringLength(254)]
    public string Email { get; set; } = null!;

    [StringLength(30)]
    public string? Phone { get; set; }

    [MaxLength(512)]
    public byte[] PasswordHash { get; set; } = null!;

    [MaxLength(256)]
    public byte[] PasswordSalt { get; set; } = null!;

    public bool IsActive { get; set; }

    [Precision(3)]
    public DateTime? LastLoginAt { get; set; }

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

    [InverseProperty("User")]
    public virtual ICollection<Certification> Certifications { get; set; } = new List<Certification>();

    [InverseProperty("User")]
    public virtual EmployeeProfile? EmployeeProfile { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<EmployeeSkill> EmployeeSkillUsers { get; set; } = new List<EmployeeSkill>();

    [InverseProperty("ValidatedByUser")]
    public virtual ICollection<EmployeeSkill> EmployeeSkillValidatedByUsers { get; set; } = new List<EmployeeSkill>();

    [ForeignKey("OrganizationId")]
    [InverseProperty("Users")]
    public virtual Organization Organization { get; set; } = null!;

    [InverseProperty("ReviewedByUser")]
    public virtual ICollection<ProjectApplication> ProjectApplicationReviewedByUsers { get; set; } = new List<ProjectApplication>();

    [InverseProperty("User")]
    public virtual ICollection<ProjectApplication> ProjectApplicationUsers { get; set; } = new List<ProjectApplication>();

    [InverseProperty("AssignedByUser")]
    public virtual ICollection<ProjectAssignment> ProjectAssignmentAssignedByUsers { get; set; } = new List<ProjectAssignment>();

    [InverseProperty("User")]
    public virtual ICollection<ProjectAssignment> ProjectAssignmentUsers { get; set; } = new List<ProjectAssignment>();

    [InverseProperty("User")]
    public virtual ICollection<ProjectParticipation> ProjectParticipations { get; set; } = new List<ProjectParticipation>();

    [InverseProperty("GeneratedByUser")]
    public virtual ICollection<RecommendationLog> RecommendationLogs { get; set; } = new List<RecommendationLog>();

    [InverseProperty("User")]
    public virtual ICollection<SkillEvaluation> SkillEvaluations { get; set; } = new List<SkillEvaluation>();

    [InverseProperty("User")]
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    [InverseProperty("User")]
    public virtual ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
}