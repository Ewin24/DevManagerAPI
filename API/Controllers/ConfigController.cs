namespace API.Controllers;

using Application.Common.Models;
using Application.DTOs.Config;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controlador de solo lectura para acceder a los catálogos/tablas de configuración del sistema.
/// Permite al frontend poblar dropdowns, filtros y validaciones con datos parametrizables.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ConfigController : ControllerBase
{
    private readonly IConfigService _configService;
    private readonly ILogger<ConfigController> _logger;

    public ConfigController(IConfigService configService, ILogger<ConfigController> logger)
    {
        _configService = configService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todos los catálogos de configuración en una sola llamada.
    /// Ideal para carga inicial del frontend.
    /// </summary>
    /// <returns>Todos los catálogos del sistema agrupados</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<AllConfigCatalogsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllCatalogs()
    {
        _logger.LogInformation("Obteniendo todos los catálogos de configuración");
        var catalogs = await _configService.GetAllCatalogsAsync();
        return Ok(ApiResponse<AllConfigCatalogsDto>.SuccessResponse(catalogs));
    }

    /// <summary>
    /// Obtiene los estados de proyecto disponibles.
    /// </summary>
    [HttpGet("project-statuses")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProjectStatusDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProjectStatuses()
    {
        var data = await _configService.GetProjectStatusesAsync();
        return Ok(ApiResponse<IEnumerable<ProjectStatusDto>>.SuccessResponse(data));
    }

    /// <summary>
    /// Obtiene los niveles de complejidad de proyecto disponibles.
    /// </summary>
    [HttpGet("complexity-levels")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProjectComplexityLevelDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetComplexityLevels()
    {
        var data = await _configService.GetComplexityLevelsAsync();
        return Ok(ApiResponse<IEnumerable<ProjectComplexityLevelDto>>.SuccessResponse(data));
    }

    /// <summary>
    /// Obtiene los estados de postulación disponibles.
    /// </summary>
    [HttpGet("application-statuses")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ApplicationStatusDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetApplicationStatuses()
    {
        var data = await _configService.GetApplicationStatusesAsync();
        return Ok(ApiResponse<IEnumerable<ApplicationStatusDto>>.SuccessResponse(data));
    }

    /// <summary>
    /// Obtiene los estados de asignación disponibles.
    /// </summary>
    [HttpGet("assignment-statuses")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<AssignmentStatusDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAssignmentStatuses()
    {
        var data = await _configService.GetAssignmentStatusesAsync();
        return Ok(ApiResponse<IEnumerable<AssignmentStatusDto>>.SuccessResponse(data));
    }

    /// <summary>
    /// Obtiene los niveles de habilidad disponibles.
    /// </summary>
    [HttpGet("skill-levels")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<SkillLevelDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSkillLevels()
    {
        var data = await _configService.GetSkillLevelsAsync();
        return Ok(ApiResponse<IEnumerable<SkillLevelDto>>.SuccessResponse(data));
    }

    /// <summary>
    /// Obtiene los puntajes de contribución disponibles.
    /// </summary>
    [HttpGet("contribution-scores")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ContributionScoreDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetContributionScores()
    {
        var data = await _configService.GetContributionScoresAsync();
        return Ok(ApiResponse<IEnumerable<ContributionScoreDto>>.SuccessResponse(data));
    }

    /// <summary>
    /// Obtiene las fuentes de evaluación disponibles.
    /// </summary>
    [HttpGet("evaluation-sources")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<EvaluationSourceDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEvaluationSources()
    {
        var data = await _configService.GetEvaluationSourcesAsync();
        return Ok(ApiResponse<IEnumerable<EvaluationSourceDto>>.SuccessResponse(data));
    }

    /// <summary>
    /// Obtiene los tipos de habilidad disponibles.
    /// </summary>
    [HttpGet("skill-types")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<SkillTypeDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSkillTypes()
    {
        var data = await _configService.GetSkillTypesAsync();
        return Ok(ApiResponse<IEnumerable<SkillTypeDto>>.SuccessResponse(data));
    }

    /// <summary>
    /// Obtiene las categorías de habilidad disponibles (con jerarquía padre-hijo).
    /// </summary>
    [HttpGet("skill-categories")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<SkillCategoryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSkillCategories()
    {
        var data = await _configService.GetSkillCategoriesAsync();
        return Ok(ApiResponse<IEnumerable<SkillCategoryDto>>.SuccessResponse(data));
    }

    /// <summary>
    /// Obtiene los tipos de acción del agente AI disponibles.
    /// </summary>
    [HttpGet("agent-action-types")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<AgentActionTypeDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAgentActionTypes()
    {
        var data = await _configService.GetAgentActionTypesAsync();
        return Ok(ApiResponse<IEnumerable<AgentActionTypeDto>>.SuccessResponse(data));
    }

    /// <summary>
    /// Obtiene los estados de acción del agente AI disponibles.
    /// </summary>
    [HttpGet("agent-action-statuses")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<AgentActionStatusDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAgentActionStatuses()
    {
        var data = await _configService.GetAgentActionStatusesAsync();
        return Ok(ApiResponse<IEnumerable<AgentActionStatusDto>>.SuccessResponse(data));
    }

    /// <summary>
    /// Obtiene los niveles de antigüedad/seniority disponibles.
    /// </summary>
    [HttpGet("seniority-levels")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<SeniorityLevelDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSeniorityLevels()
    {
        var data = await _configService.GetSeniorityLevelsAsync();
        return Ok(ApiResponse<IEnumerable<SeniorityLevelDto>>.SuccessResponse(data));
    }
}