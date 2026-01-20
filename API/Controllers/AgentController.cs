using Application.Common.Models;
using Application.DTOs.Agent;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Controlador del Agente de Orquestación de Talento (AI-powered)
/// </summary>
[ApiController]
[Route("[controller]")]
[Authorize]
public class AgentController : ControllerBase
{
    private readonly IAgentService _agentService;
    private readonly ILogger<AgentController> _logger;

    public AgentController(
        IAgentService agentService,
        ILogger<AgentController> logger)
    {
        _agentService = agentService;
        _logger = logger;
    }

    /// <summary>
    /// Consulta general al agente en lenguaje natural
    /// </summary>
    /// <remarks>
    /// Permite hacer preguntas al agente sobre talento, proyectos, skills, etc.
    /// El agente responderá con análisis inteligente y recomendaciones.
    /// 
    /// Ejemplos de consultas:
    /// - "¿Quiénes son los mejores candidatos para un proyecto de .NET?"
    /// - "Analiza las brechas de capacitación en el equipo de frontend"
    /// - "¿Qué skills están más demandadas en los proyectos activos?"
    /// </remarks>
    [HttpPost("query")]
    [ProducesResponseType(typeof(ApiResponse<AgentQueryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Query([FromBody] AgentQueryRequest request)
    {
        var organizationId = GetOrganizationId();
        
        _logger.LogInformation(
            "Usuario {UserId} consultando agente: {Query}", 
            GetUserId(), request.Query);

        var response = await _agentService.QueryAsync(organizationId, request);

        return Ok(new ApiResponse<AgentQueryResponse>
        {
            Success = true,
            Message = "Consulta procesada exitosamente",
            Data = response
        });
    }

    /// <summary>
    /// Validación semántica de habilidades con evidencia
    /// </summary>
    /// <remarks>
    /// Valida si el nivel declarado de una habilidad es coherente con:
    /// - Años de experiencia del empleado
    /// - Certificaciones relacionadas
    /// - Evidencia proporcionada
    /// 
    /// Retorna una puntuación de confianza (0-100) y recomendaciones.
    /// </remarks>
    [HttpPost("validate-skill")]
    [ProducesResponseType(typeof(ApiResponse<SkillValidationResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ValidateSkill([FromBody] SkillValidationRequest request)
    {
        var organizationId = GetOrganizationId();

        _logger.LogInformation(
            "Validando skill {SkillId} nivel {Level} para usuario {UserId}",
            request.SkillId, request.Level, request.UserId);

        var response = await _agentService.ValidateSkillAsync(organizationId, request);

        return Ok(new ApiResponse<SkillValidationResponse>
        {
            Success = true,
            Message = response.IsValid 
                ? "Skill validado exitosamente" 
                : "Skill requiere revisión",
            Data = response
        });
    }

    /// <summary>
    /// Matching inteligente de candidatos para un proyecto
    /// </summary>
    /// <remarks>
    /// Analiza todos los empleados disponibles y calcula un score de matching (0-100) 
    /// basado en:
    /// - Cumplimiento de skills obligatorias
    /// - Nivel de proficiencia en cada skill
    /// - Años de experiencia
    /// - Certificaciones relevantes
    /// - Historial de contribución en proyectos similares
    /// 
    /// Retorna los mejores candidatos ordenados por score.
    /// </remarks>
    [HttpPost("match-candidates")]
    [ProducesResponseType(typeof(ApiResponse<SkillMatchResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> MatchCandidates([FromBody] SkillMatchRequest request)
    {
        var organizationId = GetOrganizationId();

        _logger.LogInformation(
            "Matching candidatos para proyecto {ProjectId}",
            request.ProjectId);

        var response = await _agentService.MatchCandidatesForProjectAsync(
            organizationId, request);

        return Ok(new ApiResponse<SkillMatchResponse>
        {
            Success = true,
            Message = $"Se encontraron {response.Candidates.Count} candidatos",
            Data = response
        });
    }

    /// <summary>
    /// Aprobar una acción del agente (HITL - Human In The Loop)
    /// </summary>
    /// <remarks>
    /// Permite aprobar acciones críticas del agente que requieren validación humana,
    /// como asignaciones de proyectos, cambios de nivel de skills, etc.
    /// </remarks>
    [HttpPost("approve/{actionId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ApproveAction(Guid actionId)
    {
        var organizationId = GetOrganizationId();
        var userId = GetUserId();

        _logger.LogInformation(
            "Usuario {UserId} aprobando acción {ActionId}",
            userId, actionId);

        await _agentService.ApproveAgentActionAsync(organizationId, actionId, userId);

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Acción aprobada exitosamente",
            Data = null
        });
    }

    /// <summary>
    /// Rechazar una acción del agente
    /// </summary>
    [HttpPost("reject/{actionId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RejectAction(
        Guid actionId, 
        [FromBody] RejectActionRequest request)
    {
        var organizationId = GetOrganizationId();
        var userId = GetUserId();

        _logger.LogInformation(
            "Usuario {UserId} rechazando acción {ActionId}: {Reason}",
            userId, actionId, request.Reason);

        await _agentService.RejectAgentActionAsync(
            organizationId, actionId, userId, request.Reason);

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Acción rechazada",
            Data = null
        });
    }

    private Guid GetOrganizationId()
    {
        var claim = User.FindFirst("OrganizationId")?.Value;
        return Guid.Parse(claim!);
    }

    private Guid GetUserId()
    {
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(claim!);
    }
}

public record RejectActionRequest(string Reason);
