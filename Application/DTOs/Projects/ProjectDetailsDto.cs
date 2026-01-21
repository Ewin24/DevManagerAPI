namespace Application.DTOs.Projects;

using Domain.Enums;

/// <summary>
/// DTO detallado de proyecto con requisitos de skills (usado por el agente)
/// </summary>
public record ProjectDetailsDto
{
    public required Guid Id { get; init; }
    public required string Code { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public required ProjectComplexity ComplexityLevel { get; init; }
    public required ProjectStatus Status { get; init; }
    public required IEnumerable<SkillRequirementResponse> SkillRequirements { get; init; } = Array.Empty<SkillRequirementResponse>();
}
