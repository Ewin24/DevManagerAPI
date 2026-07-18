namespace Domain.Entities.Projects;

using Domain.Common;
using Domain.Entities.IAM;
using Domain.Enums;

/// <summary>
/// Proyecto con requisitos y ciclo de vida completo
/// </summary>
public class Project : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public string? Code { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public ProjectComplexity ComplexityLevel { get; set; }
    public ProjectStatus Status { get; set; }

    // Navegación
    public Organization? Organization { get; set; }
}
