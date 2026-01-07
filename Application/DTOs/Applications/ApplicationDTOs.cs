namespace Application.DTOs.Applications;

using System.ComponentModel.DataAnnotations;
using Domain.Enums;

/// <summary>
/// DTO para postular a un proyecto
/// </summary>
public class ApplyToProjectRequest
{
    [MaxLength(800)]
    public string? Message { get; set; }
}

/// <summary>
/// DTO de respuesta de postulación
/// </summary>
public class ApplicationResponse
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = null!;
    public Guid UserId { get; set; }
    public string UserName { get; set; } = null!;
    public string? Message { get; set; }
    public ApplicationStatus Status { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewNotes { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// DTO para revisar postulación (aprobar/rechazar)
/// </summary>
public class ReviewApplicationRequest
{
    [Required]
    public ApplicationStatus Status { get; set; } // Approved o Rejected

    [MaxLength(500)]
    public string? ReviewNotes { get; set; }
}
