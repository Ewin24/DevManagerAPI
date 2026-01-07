using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Entities;

[Table("ProjectRoles", Schema = "projects")]
public partial class ProjectRole
{
    [Key]
    public Guid Id { get; set; }

    public Guid OrganizationId { get; set; }

    public Guid ProjectId { get; set; }

    [StringLength(80)]
    public string Name { get; set; } = null!;

    public int NeededCount { get; set; }

    [Precision(3)]
    public DateTime CreatedAt { get; set; }

    public bool IsDeleted { get; set; }

    [ForeignKey("OrganizationId")]
    [InverseProperty("ProjectRoles")]
    public virtual Organization Organization { get; set; } = null!;

    [ForeignKey("ProjectId")]
    [InverseProperty("ProjectRoles")]
    public virtual Project Project { get; set; } = null!;

    [InverseProperty("ProjectRole")]
    public virtual ICollection<ProjectAssignment> ProjectAssignments { get; set; } = new List<ProjectAssignment>();
}
