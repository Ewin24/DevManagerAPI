namespace Domain.Entities.Agent;

/// <summary>
/// Representa una herramienta cooperativa disponible para el Asistente Cooperativo IA
/// Asociada a normatividad Supersolidaria, balance social, cumplimiento y atención al asociado
/// </summary>
public class HerramientaCooperativa
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public string? InputSchema { get; set; } // JSON Schema de parámetros de entrada
    public string? UrlEndpoint { get; set; } // Endpoint externo opcional (Gemini, Supersolidaria)
    public bool Activa { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
