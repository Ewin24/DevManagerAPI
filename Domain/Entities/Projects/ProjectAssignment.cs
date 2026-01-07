namespace Domain.Entities.Projects;

using Domain.Entities.IAM;
using Domain.Enums;

/// <summary>
/// Asignación activa de un empleado a un proyecto
/// </summary>
public class ProjectAssignment
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid UserId { get; set; }
    public Guid? ProjectRoleId { get; set; }
    public Guid AssignedByUserId { get; set; }
    public DateTime AssignedAt { get; set; }
    public AssignmentStatus Status { get; set; }
    public DateTime? EndedAt { get; set; }
    public bool IsDeleted { get; set; }

    // Navegación
    public Organization? Organization { get; set; }
    public Project? Project { get; set; }
    public User? User { get; set; }
    public ProjectRole? ProjectRole { get; set; }
    public User? AssignedBy { get; set; }
}
