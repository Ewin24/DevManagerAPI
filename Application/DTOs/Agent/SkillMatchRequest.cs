namespace Application.DTOs.Agent;

/// <summary>
/// Solicitud de matching de habilidades para un proyecto
/// </summary>
public record SkillMatchRequest
{
    public required Guid ProjectId { get; init; }
    public int? MaxCandidates { get; init; } = 10;
    public bool IncludeReasoningDetails { get; init; } = true;
}
