namespace Application.DTOs.Profiles;

using Application.DTOs.Skills;

/// <summary>
/// DTO para perfil de empleado con sus habilidades (usado por el agente)
/// </summary>
public record ProfileWithSkillsDto
{
    public required Guid UserId { get; init; }
    public string? Bio { get; init; }
    public byte? YearsExperience { get; init; }
    public string? LinkedInUrl { get; init; }
    public string? PortfolioUrl { get; init; }
    public required IEnumerable<EmployeeSkillSummary> Skills { get; init; } = Array.Empty<EmployeeSkillSummary>();
}

/// <summary>
/// Resumen de habilidad del empleado
/// </summary>
public record EmployeeSkillSummary
{
    public required Guid SkillId { get; init; }
    public required string SkillName { get; init; }
    public required byte CurrentLevel { get; init; } // 1-5
    public DateTime? LastValidatedAt { get; init; }
}
