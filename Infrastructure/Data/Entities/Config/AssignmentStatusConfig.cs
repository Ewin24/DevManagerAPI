using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Data.Entities.Config;

/// <summary>
/// Entidad EF Core para config.AssignmentStatuses
/// </summary>
[Table("AssignmentStatuses", Schema = "config")]
public class AssignmentStatusConfig
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

    public bool IsFinalState { get; set; }

    public byte DisplayOrder { get; set; }

    public bool IsActive { get; set; } = true;

    // Navegación inversa
    [InverseProperty("AssignmentStatusNavigation")]
    public virtual ICollection<ProjectAssignment> ProjectAssignments { get; set; } = new List<ProjectAssignment>();
}