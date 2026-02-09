using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Data.Entities.Config;

/// <summary>
/// Entidad EF Core para config.ProjectStatuses
/// </summary>
[Table("ProjectStatuses", Schema = "config")]
public class ProjectStatusConfig
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

    public byte DisplayOrder { get; set; }

    public bool IsActive { get; set; } = true;

    public bool AllowsApplications { get; set; }

    // Navegación inversa
    [InverseProperty("ProjectStatusNavigation")]
    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
}