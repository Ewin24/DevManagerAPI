using Infrastructure.Data.Entities.Config;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Data.Entities;

[Table("ProjectSkillRequirements", Schema = "projects")]
public partial class ProjectSkillRequirement
{
    [Key]
    public Guid Id { get; set; }

    public Guid OrganizationId { get; set; }

    public Guid ProjectId { get; set; }

    public Guid SkillId { get; set; }

    public byte RequiredLevel { get; set; }

    public bool IsMandatory { get; set; }

    [Precision(3)]
    public DateTime CreatedAt { get; set; }

    public bool IsDeleted { get; set; }

    [ForeignKey("OrganizationId")]
    [InverseProperty("ProjectSkillRequirements")]
    public virtual Organization Organization { get; set; } = null!;

    [ForeignKey("ProjectId")]
    [InverseProperty("ProjectSkillRequirements")]
    public virtual Project Project { get; set; } = null!;

    [ForeignKey("SkillId")]
    [InverseProperty("ProjectSkillRequirements")]
    public virtual Skill Skill { get; set; } = null!;

    [ForeignKey("RequiredLevel")]
    public virtual SkillLevelConfig RequiredLevelNavigation { get; set; } = null!;
}