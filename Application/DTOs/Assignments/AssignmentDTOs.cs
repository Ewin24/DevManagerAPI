namespace Application.DTOs.Assignments;

using System.ComponentModel.DataAnnotations;
using Domain.Enums;

/// <summary>
/// DTO para asignar empleado a proyecto
/// </summary>
public class CreateAssignmentRequest
{
    [Required]
    public Guid ProjectId { get; set; }

    [Required]
    public Guid UserId { get; set; }

    public Guid? ProjectRoleId { get; set; }

    /// <summary>
    /// ID de la aplicación previa (opcional)
    /// </summary>
    public Guid? ApplicationId { get; set; }
}

/// <summary>
/// DTO de respuesta de asignación
/// </summary>
public class AssignmentResponse
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = null!;
    public Guid UserId { get; set; }
    public string UserName { get; set; } = null!;
    public string? RoleName { get; set; }
    public AssignmentStatus Status { get; set; }
    public DateTime AssignedAt { get; set; }
    public DateTime? EndedAt { get; set; }
}
