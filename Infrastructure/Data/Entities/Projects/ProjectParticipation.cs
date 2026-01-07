using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Entities;

[Table("ProjectParticipation", Schema = "projects")]
public partial class ProjectParticipation
{
    [Key]
    public Guid Id { get; set; }

    public Guid OrganizationId { get; set; }

    public Guid ProjectId { get; set; }

    public Guid UserId { get; set; }

    [StringLength(80)]
    public string? RoleName { get; set; }

    public byte? ContributionScore { get; set; }

    public string? FeedbackComments { get; set; }

    [Precision(3)]
    public DateTime? CompletedAt { get; set; }

    [Precision(3)]
    public DateTime CreatedAt { get; set; }

    public bool IsDeleted { get; set; }

    [ForeignKey("OrganizationId")]
    [InverseProperty("ProjectParticipations")]
    public virtual Organization Organization { get; set; } = null!;

    [ForeignKey("ProjectId")]
    [InverseProperty("ProjectParticipations")]
    public virtual Project Project { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("ProjectParticipations")]
    public virtual User User { get; set; } = null!;
}
