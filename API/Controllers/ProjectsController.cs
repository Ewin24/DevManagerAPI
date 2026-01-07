namespace API.Controllers;

using Application.Common.Exceptions;
using Application.Common.Models;
using Application.DTOs.Projects;
using Application.Interfaces;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

/// <summary>
/// Controller para gestión de proyectos
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;
    private readonly ILogger<ProjectsController> _logger;

    public ProjectsController(IProjectService projectService, ILogger<ProjectsController> logger)
    {
        _projectService = projectService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todos los proyectos con filtros opcionales
    /// </summary>
    /// <param name="status">Filtro por estado del proyecto</param>
    /// <returns>Lista de proyectos</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProjectResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] ProjectStatus? status = null)
    {
        var organizationId = Guid.Parse(User.FindFirst("OrganizationId")?.Value ?? string.Empty);

        var projects = await _projectService.GetAllProjectsAsync(organizationId, status);

        return Ok(ApiResponse<IEnumerable<ProjectResponse>>.SuccessResponse(projects));
    }

    /// <summary>
    /// Obtiene un proyecto por ID
    /// </summary>
    /// <param name="id">ID del proyecto</param>
    /// <returns>Datos del proyecto</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ProjectResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var organizationId = Guid.Parse(User.FindFirst("OrganizationId")?.Value ?? string.Empty);

        var project = await _projectService.GetProjectByIdAsync(id, organizationId);

        if (project == null)
        {
            return NotFound();
        }

        return Ok(ApiResponse<ProjectResponse>.SuccessResponse(project));
    }

    /// <summary>
    /// Crea un nuevo proyecto
    /// </summary>
    /// <param name="request">Datos del proyecto</param>
    /// <returns>ID del proyecto creado</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateProjectRequest request)
    {
        var organizationId = Guid.Parse(User.FindFirst("OrganizationId")?.Value ?? string.Empty);

        var projectId = await _projectService.CreateProjectAsync(request, organizationId);

        return CreatedAtAction(
            nameof(GetById),
            new { id = projectId },
            ApiResponse<Guid>.SuccessResponse(projectId, "Proyecto creado exitosamente"));
    }

    /// <summary>
    /// Agrega un requisito de habilidad al proyecto
    /// </summary>
    /// <param name="id">ID del proyecto</param>
    /// <param name="request">Datos del requisito</param>
    /// <returns>ID del requisito creado</returns>
    [HttpPost("{id}/reqs")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddSkillRequirement(Guid id, [FromBody] AddSkillRequirementRequest request)
    {
        var organizationId = Guid.Parse(User.FindFirst("OrganizationId")?.Value ?? string.Empty);

        try
        {
            var requirementId = await _projectService.AddSkillRequirementAsync(id, organizationId, request);

            return CreatedAtAction(
                nameof(GetSkillRequirements),
                new { id },
                ApiResponse<Guid>.SuccessResponse(requirementId, "Requisito agregado exitosamente"));
        }
        catch (NotFoundException ex)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Obtiene los requisitos de habilidades de un proyecto
    /// </summary>
    /// <param name="id">ID del proyecto</param>
    /// <returns>Lista de requisitos</returns>
    [HttpGet("{id}/reqs")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<SkillRequirementResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSkillRequirements(Guid id)
    {
        var organizationId = Guid.Parse(User.FindFirst("OrganizationId")?.Value ?? string.Empty);

        var requirements = await _projectService.GetSkillRequirementsAsync(id, organizationId);

        return Ok(ApiResponse<IEnumerable<SkillRequirementResponse>>.SuccessResponse(requirements));
    }
}
