namespace Domain.Entities.Projects;

using Domain.Entities.IAM;

/// <summary>
/// Roles específicos dentro de un proyecto
/// </summary>
public class ProjectRole
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = null!; // Dev, QA, PM, etc.
    public int NeededCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsDeleted { get; set; }

    // Navegación
    public Organization? Organization { get; set; }
    public Project? Project { get; set; }
}
