namespace Domain.Entities.Agent;

/// <summary>
/// Configuración del agente por organización
/// </summary>
public class AgentConfiguration
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public required string ConfigKey { get; set; } // "CONFIDENCE_THRESHOLD", "AUTO_APPROVE", etc.
    public required string ConfigValue { get; set; } // JSON o texto
    public DateTime UpdatedAt { get; set; }
}
