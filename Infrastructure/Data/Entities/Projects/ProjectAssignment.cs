using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Entities;

[Table("ProjectAssignments", Schema = "projects")]
public partial class ProjectAssignment
{
    [Key]
    public Guid Id { get; set; }

    public Guid OrganizationId { get; set; }

    public Guid ProjectId { get; set; }

    public Guid UserId { get; set; }

    public Guid? ProjectRoleId { get; set; }

    public Guid AssignedByUserId { get; set; }

    [Precision(3)]
    public DateTime AssignedAt { get; set; }

    public byte Status { get; set; }

    [Precision(3)]
    public DateTime? EndedAt { get; set; }

    public bool IsDeleted { get; set; }

    [ForeignKey("AssignedByUserId")]
    [InverseProperty("ProjectAssignmentAssignedByUsers")]
    public virtual User AssignedByUser { get; set; } = null!;

    [ForeignKey("OrganizationId")]
    [InverseProperty("ProjectAssignments")]
    public virtual Organization Organization { get; set; } = null!;

    [ForeignKey("ProjectId")]
    [InverseProperty("ProjectAssignments")]
    public virtual Project Project { get; set; } = null!;

    [ForeignKey("ProjectRoleId")]
    [InverseProperty("ProjectAssignments")]
    public virtual ProjectRole? ProjectRole { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("ProjectAssignmentUsers")]
    public virtual User User { get; set; } = null!;
}
