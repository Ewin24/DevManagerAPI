using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Data.Entities.Config;

/// <summary>
/// Entidad EF Core para config.ApplicationStatuses
/// </summary>
[Table("ApplicationStatuses", Schema = "config")]
public class ApplicationStatusConfig
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

    public bool RequiresReviewNotes { get; set; }

    public bool IsFinalState { get; set; }

    public byte DisplayOrder { get; set; }

    public bool IsActive { get; set; } = true;

    // Navegación inversa
    [InverseProperty("ApplicationStatusNavigation")]
    public virtual ICollection<ProjectApplication> ProjectApplications { get; set; } = new List<ProjectApplication>();
}