using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Entities;

[Table("ProjectApplications", Schema = "projects")]
public partial class ProjectApplication
{
    [Key]
    public Guid Id { get; set; }

    public Guid OrganizationId { get; set; }

    public Guid ProjectId { get; set; }

    public Guid UserId { get; set; }

    [StringLength(800)]
    public string? Motivation { get; set; }

    public byte Status { get; set; }

    public Guid? ReviewedByUserId { get; set; }

    [Precision(3)]
    public DateTime? ReviewedAt { get; set; }

    [StringLength(500)]
    public string? ReviewNotes { get; set; }

    [Precision(3)]
    public DateTime CreatedAt { get; set; }

    public bool IsDeleted { get; set; }

    [ForeignKey("OrganizationId")]
    [InverseProperty("ProjectApplications")]
    public virtual Organization Organization { get; set; } = null!;

    [ForeignKey("ProjectId")]
    [InverseProperty("ProjectApplications")]
    public virtual Project Project { get; set; } = null!;

    [ForeignKey("ReviewedByUserId")]
    [InverseProperty("ProjectApplicationReviewedByUsers")]
    public virtual User? ReviewedByUser { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("ProjectApplicationUsers")]
    public virtual User User { get; set; } = null!;
}
