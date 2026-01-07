namespace Domain.Entities.Projects;

using Domain.Entities.IAM;
using Domain.Enums;

/// <summary>
/// Postulación de un empleado a un proyecto
/// </summary>
public class ProjectApplication
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid UserId { get; set; }
    public string? Motivation { get; set; }
    public ApplicationStatus Status { get; set; }
    public Guid? ReviewedByUserId { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewNotes { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsDeleted { get; set; }

    // Navegación
    public Organization? Organization { get; set; }
    public Project? Project { get; set; }
    public User? User { get; set; }
    public User? ReviewedBy { get; set; }
}
