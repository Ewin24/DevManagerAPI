using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Data.Entities.Config;

/// <summary>
/// Entidad EF Core para config.ContributionScores
/// </summary>
[Table("ContributionScores", Schema = "config")]
public class ContributionScoreConfig
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public byte Id { get; set; }

    [Required]
    [StringLength(20)]
    public string Code { get; set; } = null!;

    [Required]
    [StringLength(80)]
    public string Name { get; set; } = null!;

    [StringLength(200)]
    public string? Description { get; set; }

    [Column(TypeName = "decimal(3,2)")]
    public decimal ExperienceBonus { get; set; }

    public byte DisplayOrder { get; set; }

    public bool IsActive { get; set; } = true;

    // Navegación inversa
    [InverseProperty("ContributionScoreNavigation")]
    public virtual ICollection<ProjectParticipation> ProjectParticipations { get; set; } = new List<ProjectParticipation>();
}