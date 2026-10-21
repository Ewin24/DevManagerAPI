namespace Application.DTOs.Assignments;

using Domain.Enums;
using System.ComponentModel.DataAnnotations;

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
/// DTO para actualizar una asignación (campos mutables de CreateAssignmentRequest)
/// </summary>
public class UpdateAssignmentRequest
{
    /// <summary>
    /// Rol dentro del proyecto (null = sin rol)
    /// </summary>
    public Guid? ProjectRoleId { get; set; }
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
