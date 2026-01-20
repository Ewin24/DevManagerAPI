namespace Application.DTOs.Agent;

/// <summary>
/// Solicitud de validación semántica de habilidades
/// </summary>
public record SkillValidationRequest
{
    public required Guid UserId { get; init; }
    public required Guid SkillId { get; init; }
    public required int Level { get; init; } // 1-5
    public string? EvidenceUrl { get; init; }
    public List<Guid>? RelatedCertificationIds { get; init; }
}
