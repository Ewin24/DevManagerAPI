using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Entities;

[Table("RecommendationRules", Schema = "reporting")]
public partial class RecommendationRule
{
    [Key]
    public Guid Id { get; set; }

    public Guid OrganizationId { get; set; }

    [StringLength(120)]
    public string Name { get; set; } = null!;

    [StringLength(800)]
    public string ConditionExpr { get; set; } = null!;

    [StringLength(800)]
    public string RecommendationText { get; set; } = null!;

    public bool IsActive { get; set; }

    [Precision(3)]
    public DateTime CreatedAt { get; set; }

    [Precision(3)]
    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    [ForeignKey("OrganizationId")]
    [InverseProperty("RecommendationRules")]
    public virtual Organization Organization { get; set; } = null!;
}
