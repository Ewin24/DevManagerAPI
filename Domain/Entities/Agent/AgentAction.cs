namespace Domain.Entities.Agent;

/// <summary>
/// Representa una acción realizada por el agente (auditoría)
/// </summary>
public class AgentAction
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public required string ActionType { get; set; } // "SKILL_VALIDATION", "PROJECT_MATCHING", etc.
    public required string Description { get; set; }
    public required string InputData { get; set; } // JSON
    public required string OutputData { get; set; } // JSON
    public required string Status { get; set; } // "SUCCESS", "FAILED", "PENDING_APPROVAL"
    public Guid? ExecutedByUserId { get; set; } // Null si es automático
    public Guid? ApprovedByUserId { get; set; } // HITL
    public DateTime CreatedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
}
