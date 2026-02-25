using Infrastructure.Data.Entities.Config;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Data.Entities;

[Table("EmployeeSkills", Schema = "talent")]
public partial class EmployeeSkill
{
    [Key]
    public Guid Id { get; set; }

    public Guid OrganizationId { get; set; }

    public Guid UserId { get; set; }

    public Guid SkillId { get; set; }

    public byte Level { get; set; }

    [StringLength(400)]
    public string? EvidenceUrl { get; set; }

    [Precision(3)]
    public DateTime? LastValidatedAt { get; set; }

    public Guid? ValidatedByUserId { get; set; }

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

    [ForeignKey("OrganizationId")]
    [InverseProperty("EmployeeSkills")]
    public virtual Organization Organization { get; set; } = null!;

    [ForeignKey("SkillId")]
    [InverseProperty("EmployeeSkills")]
    public virtual Skill Skill { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("EmployeeSkillUsers")]
    public virtual User User { get; set; } = null!;

    [ForeignKey("ValidatedByUserId")]
    [InverseProperty("EmployeeSkillValidatedByUsers")]
    public virtual User? ValidatedByUser { get; set; }

    [ForeignKey("Level")]
    public virtual SkillLevelConfig SkillLevelNavigation { get; set; } = null!;
}