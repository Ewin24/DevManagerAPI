using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Data.Entities.Config;

/// <summary>
/// Entidad EF Core para config.SeniorityLevels
/// </summary>
[Table("SeniorityLevels", Schema = "config")]
public class SeniorityLevelConfig
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public byte Id { get; set; }

    [Required]
    [StringLength(20)]
    public string Code { get; set; } = null!;

    [Required]
    [StringLength(80)]
    public string Name { get; set; } = null!;

    [StringLength(200)]
    public string? Description { get; set; }

    public byte MinYearsExperience { get; set; }

    public byte? MaxYearsExperience { get; set; }

    public byte DisplayOrder { get; set; }

    public bool IsActive { get; set; } = true;
}