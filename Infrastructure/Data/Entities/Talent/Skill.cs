using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Data.Entities;

[Table("Skills", Schema = "talent")]
public partial class Skill
{
    [Key]
    public Guid Id { get; set; }

    public Guid? OrganizationId { get; set; }

    [StringLength(120)]
    public string Name { get; set; } = null!;

    [StringLength(80)]
    public string? Category { get; set; }

    [StringLength(20)]
    public string? SkillType { get; set; }

    [Precision(3)]
    public DateTime CreatedAt { get; set; }

    public Guid? CreatedByUserId { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedByUserId { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedByUserId { get; set; }

    [InverseProperty("Skill")]
    public virtual ICollection<EmployeeSkill> EmployeeSkills { get; set; } = new List<EmployeeSkill>();

    [ForeignKey("OrganizationId")]
    [InverseProperty("Skills")]
    public virtual Organization? Organization { get; set; }

    [InverseProperty("Skill")]
    public virtual ICollection<ProjectSkillRequirement> ProjectSkillRequirements { get; set; } = new List<ProjectSkillRequirement>();

    [InverseProperty("Skill")]
    public virtual ICollection<SkillEvaluation> SkillEvaluations { get; set; } = new List<SkillEvaluation>();
}
