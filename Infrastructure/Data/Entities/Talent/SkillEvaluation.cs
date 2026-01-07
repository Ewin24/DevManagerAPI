using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Entities;

[Table("SkillEvaluations", Schema = "talent")]
[Index("OrganizationId", "UserId", "SkillId", "CreatedAt", Name = "IX_SkillEvaluations_Org_User_Skill_Date", IsDescending = new[] { false, false, false, true })]
public partial class SkillEvaluation
{
    [Key]
    public Guid Id { get; set; }

    public Guid OrganizationId { get; set; }

    public Guid UserId { get; set; }

    public Guid SkillId { get; set; }

    public byte Source { get; set; }

    public Guid? ProjectId { get; set; }

    public short DeltaLevel { get; set; }

    [StringLength(400)]
    public string? Reason { get; set; }

    [Precision(3)]
    public DateTime CreatedAt { get; set; }

    public Guid? CreatedByUserId { get; set; }

    [ForeignKey("OrganizationId")]
    [InverseProperty("SkillEvaluations")]
    public virtual Organization Organization { get; set; } = null!;

    [ForeignKey("ProjectId")]
    [InverseProperty("SkillEvaluations")]
    public virtual Project? Project { get; set; }

    [ForeignKey("SkillId")]
    [InverseProperty("SkillEvaluations")]
    public virtual Skill Skill { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("SkillEvaluations")]
    public virtual User User { get; set; } = null!;
}
