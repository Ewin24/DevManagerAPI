namespace Application.DTOs.Skills;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// DTO para crear/actualizar habilidad en catálogo
/// </summary>
public class SkillDto
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(120)]
    public string Name { get; set; } = null!;

    [MaxLength(80)]
    public string? Category { get; set; }

    [MaxLength(20)]
    public string? SkillType { get; set; } // 'Hard', 'Soft', 'Language'
}

/// <summary>
/// DTO para asignar/actualizar habilidad de empleado
/// </summary>
public class UpsertEmployeeSkillRequest
{
    [Required]
    public Guid SkillId { get; set; }

    [Range(1, 5, ErrorMessage = "El nivel debe estar entre 1 y 5")]
    public byte Level { get; set; }

    [Url]
    [MaxLength(400)]
    public string? EvidenceUrl { get; set; }
}

/// <summary>
/// DTO de respuesta con habilidades del empleado
/// </summary>
public class EmployeeSkillResponse
{
    public Guid Id { get; set; }
    public Guid SkillId { get; set; }
    public string SkillName { get; set; } = null!;
    public string? Category { get; set; }
    public byte Level { get; set; }
    public string? EvidenceUrl { get; set; }
    public DateTime? LastValidatedAt { get; set; }
    public Guid? ValidatedByUserId { get; set; }
}

/// <summary>
/// DTO para validar habilidad
/// </summary>
public class ValidateSkillRequest
{
    [Range(1, 5)]
    public byte? NewLevel { get; set; } // Opcional: actualizar nivel al validar
}
