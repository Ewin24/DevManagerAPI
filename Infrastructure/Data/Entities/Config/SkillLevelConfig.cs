using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Data.Entities.Config;

/// <summary>
/// Entidad EF Core para config.SkillLevels
/// </summary>
[Table("SkillLevels", Schema = "config")]
public class SkillLevelConfig
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

    [StringLength(400)]
    public string? Description { get; set; }

    public byte? MinYearsExperience { get; set; }

    public byte DisplayOrder { get; set; }

    public bool IsActive { get; set; } = true;

    // Navegación inversa
    [InverseProperty("SkillLevelNavigation")]
    public virtual ICollection<EmployeeSkill> EmployeeSkills { get; set; } = new List<EmployeeSkill>();

    [InverseProperty("RequiredLevelNavigation")]
    public virtual ICollection<ProjectSkillRequirement> ProjectSkillRequirements { get; set; } = new List<ProjectSkillRequirement>();
}