namespace Application.DTOs.Projects;

using System.ComponentModel.DataAnnotations;
using Domain.Enums;

/// <summary>
/// DTO para crear proyecto
/// </summary>
public class CreateProjectRequest
{
    [MaxLength(40)]
    public string? Code { get; set; }

    [Required]
    [MaxLength(160)]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    [Required]
    public ProjectComplexity ComplexityLevel { get; set; } = ProjectComplexity.Medium;
}

/// <summary>
/// DTO de respuesta con datos del proyecto
/// </summary>
public class ProjectResponse
{
    public Guid Id { get; set; }
    public string? Code { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public ProjectComplexity ComplexityLevel { get; set; }
    public ProjectStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// DTO para actualizar proyecto
/// </summary>
public class UpdateProjectRequest
{
    [MaxLength(40)]
    public string? Code { get; set; }

    [MaxLength(160)]
    public string? Name { get; set; }

    public string? Description { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public ProjectComplexity? ComplexityLevel { get; set; }

    public ProjectStatus? Status { get; set; }
}

/// <summary>
/// DTO para agregar requisito de habilidad al proyecto
/// </summary>
public class AddSkillRequirementRequest
{
    [Required]
    public Guid SkillId { get; set; }

    [Range(1, 5)]
    public byte RequiredLevel { get; set; }

    public bool IsMandatory { get; set; } = true;
}

/// <summary>
/// DTO de requisito de habilidad
/// </summary>
public class SkillRequirementResponse
{
    public Guid Id { get; set; }
    public Guid SkillId { get; set; }
    public string SkillName { get; set; } = null!;
    public byte RequiredLevel { get; set; }
    public bool IsMandatory { get; set; }
}