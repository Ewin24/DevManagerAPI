using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Entities;

[Table("EmployeeProfiles", Schema = "talent")]
public partial class EmployeeProfile
{
    [Key]
    public Guid UserId { get; set; }

    public Guid OrganizationId { get; set; }

    [StringLength(800)]
    public string? Bio { get; set; }

    public int? YearsExperience { get; set; }

    [StringLength(300)]
    public string? LinkedInUrl { get; set; }

    [StringLength(300)]
    public string? PortfolioUrl { get; set; }

    [Precision(3)]
    public DateTime CreatedAt { get; set; }

    public Guid? CreatedByUserId { get; set; }

    [Precision(3)]
    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedByUserId { get; set; }

    public bool IsDeleted { get; set; }

    [ForeignKey("OrganizationId")]
    [InverseProperty("EmployeeProfiles")]
    public virtual Organization Organization { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("EmployeeProfile")]
    public virtual User User { get; set; } = null!;
}
