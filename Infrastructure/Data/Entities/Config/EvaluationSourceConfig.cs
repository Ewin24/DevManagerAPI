using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Data.Entities.Config;

/// <summary>
/// Entidad EF Core para config.EvaluationSources
/// </summary>
[Table("EvaluationSources", Schema = "config")]
public class EvaluationSourceConfig
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

    public bool IsAutomated { get; set; }

    public byte DisplayOrder { get; set; }

    public bool IsActive { get; set; } = true;

    // Navegación inversa
    [InverseProperty("EvaluationSourceNavigation")]
    public virtual ICollection<SkillEvaluation> SkillEvaluations { get; set; } = new List<SkillEvaluation>();
}