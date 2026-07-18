namespace Application.DTOs.Agent;

/// <summary>
/// Resultado del matching de habilidades
/// </summary>
public record SkillMatchResponse
{
    public required Guid ProjectId { get; init; }
    public required string ProjectName { get; init; }
    public List<CandidateMatch> Candidates { get; init; } = new();
    public required string AnalysisNarrative { get; init; } // Explicación del agente
}

public record CandidateMatch
{
    public required Guid UserId { get; init; }
    public required string FullName { get; init; }
    public required string Email { get; init; }
    public required double MatchScore { get; init; } // 0-100
    public List<SkillAlignment> SkillAlignments { get; init; } = new();
    public required string RecommendationReason { get; init; }
}

public record SkillAlignment
{
    public required string SkillName { get; init; }
    public required int RequiredLevel { get; init; }
    public required int CurrentLevel { get; init; }
    public required bool IsMandatory { get; init; }
    public required bool Meets { get; init; }
}
