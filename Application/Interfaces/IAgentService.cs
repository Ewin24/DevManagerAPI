using Application.DTOs.Agent;

namespace Application.Interfaces;

/// <summary>
/// Interfaz del Agente de Orquestación de Talento (patrón Tool Use + MCP)
/// </summary>
public interface IAgentService
{
    /// <summary>
    /// Consulta general al agente en lenguaje natural
    /// </summary>
    Task<AgentQueryResponse> QueryAsync(
        Guid organizationId, 
        AgentQueryRequest request, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validación semántica de habilidades con evidencia
    /// </summary>
    Task<SkillValidationResponse> ValidateSkillAsync(
        Guid organizationId, 
        SkillValidationRequest request, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Matching inteligente de proyectos (tiempo real)
    /// </summary>
    Task<SkillMatchResponse> MatchCandidatesForProjectAsync(
        Guid organizationId, 
        SkillMatchRequest request, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Aprobar una acción del agente (HITL)
    /// </summary>
    Task ApproveAgentActionAsync(
        Guid organizationId, 
        Guid actionId, 
        Guid approvedByUserId, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Rechazar una acción del agente
    /// </summary>
    Task RejectAgentActionAsync(
        Guid organizationId, 
        Guid actionId, 
        Guid rejectedByUserId, 
        string reason, 
        CancellationToken cancellationToken = default);
}
