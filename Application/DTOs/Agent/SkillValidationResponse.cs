namespace Application.DTOs.Agent;

/// <summary>
/// Resultado de la validación de habilidades
/// </summary>
public record SkillValidationResponse
{
    public required bool IsValid { get; init; }
    public required double ConfidenceScore { get; init; } // 0-100
    public required string ValidationReasoning { get; init; }
    public List<string> Recommendations { get; init; } = new();
    public List<string> Warnings { get; init; } = new();
    public bool RequiresCertification { get; init; }
}
