using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Entities;

[Table("RecommendationLogs", Schema = "reporting")]
[Index("OrganizationId", "GeneratedAt", Name = "IX_RecommendationLogs_Org_Date", IsDescending = new[] { false, true })]
public partial class RecommendationLog
{
    [Key]
    public Guid Id { get; set; }

    public Guid OrganizationId { get; set; }

    [Precision(3)]
    public DateTime GeneratedAt { get; set; }

    public Guid? GeneratedByUserId { get; set; }

    public string ResultJson { get; set; } = null!;

    [ForeignKey("GeneratedByUserId")]
    [InverseProperty("RecommendationLogs")]
    public virtual User? GeneratedByUser { get; set; }

    [ForeignKey("OrganizationId")]
    [InverseProperty("RecommendationLogs")]
    public virtual Organization Organization { get; set; } = null!;
}
