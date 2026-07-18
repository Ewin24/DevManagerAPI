namespace Application.DTOs.Agent;

/// <summary>
/// Solicitud al agente en lenguaje natural
/// </summary>
public record AgentQueryRequest
{
    public required string Query { get; init; }
    public string? Context { get; init; } // Contexto adicional opcional
    public bool RequireApproval { get; init; } = true; // HITL por defecto
}
