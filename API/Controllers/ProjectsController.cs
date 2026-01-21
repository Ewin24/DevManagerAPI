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
    /// <remarks>
    /// Retorna el listado de proyectos de la organización, con opción de filtrar por estado.
    /// 
    /// **Ejemplo de Request sin filtro:**
    /// 
    ///     GET /api/projects
    ///     Authorization: Bearer {token}
    /// 
    /// **Ejemplo de Request con filtro:**
    /// 
    ///     GET /api/projects?status=1
    ///     Authorization: Bearer {token}
    /// 
    /// **Estados de Proyecto (status):**
    /// - 0 = Draft (borrador)
    /// - 1 = Active (activo)
    /// - 2 = OnHold (en pausa)
    /// - 3 = Completed (completado)
    /// - 4 = Cancelled (cancelado)
    /// 
    /// **Ejemplo de Response (200 OK):**
    /// 
    ///     {
    ///         "success": true,
    ///         "message": null,
    ///         "data": [
    ///             {
    ///                 "id": "eeeeeeee-0000-0000-0000-000000000001",
    ///                 "code": "SIST-HOSP-001",
    ///                 "name": "Sistema de Gestión Hospitalaria",
    ///                 "description": "Modernización del sistema de registro de pacientes",
    ///                 "status": 1,
    ///                 "statusName": "Active",
    ///                 "startDate": "2024-03-01T00:00:00Z",
    ///                 "endDate": "2024-12-31T00:00:00Z",
    ///                 "complexity": 2,
    ///                 "complexityName": "High",
    ///                 "createdAt": "2024-02-15T10:00:00Z"
    ///             }
    ///         ]
    ///     }
    /// 
    /// **Casos de uso:**
    /// - Dashboard de proyectos activos
    /// - Listado de oportunidades laborales
    /// - Filtro de proyectos por estado
    /// </remarks>
    /// <param name="status">Filtro opcional por estado del proyecto (0=Draft, 1=Active, 2=OnHold, 3=Completed, 4=Cancelled)</param>
    /// <returns>Lista de proyectos</returns>
    /// <response code="200">Proyectos obtenidos exitosamente</response>
    /// <response code="401">No autenticado</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProjectResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] ProjectStatus? status = null)
    {
        var organizationId = Guid.Parse(User.FindFirst("OrganizationId")?.Value ?? string.Empty);

        var projects = await _projectService.GetAllProjectsAsync(organizationId, status);

        return Ok(ApiResponse<IEnumerable<ProjectResponse>>.SuccessResponse(projects));
    }

    /// <summary>
    /// Obtiene los detalles completos de un proyecto específico
    /// </summary>
    /// <remarks>
    /// Retorna información detallada del proyecto incluyendo descripción completa, presupuesto y fechas.
    /// 
    /// **Ejemplo de Request:**
    /// 
    ///     GET /api/projects/eeeeeeee-0000-0000-0000-000000000001
    ///     Authorization: Bearer {token}
    /// 
    /// **Ejemplo de Response (200 OK):**
    /// 
    ///     {
    ///         "success": true,
    ///         "message": null,
    ///         "data": {
    ///             "id": "eeeeeeee-0000-0000-0000-000000000001",
    ///             "code": "SIST-HOSP-001",
    ///             "name": "Sistema de Gestión Hospitalaria",
    ///             "description": "Sistema integral para gestión de pacientes, citas, historiales médicos y facturación",
    ///             "status": 1,
    ///             "statusName": "Active",
    ///             "startDate": "2024-03-01T00:00:00Z",
    ///             "endDate": "2024-12-31T00:00:00Z",
    ///             "complexity": 2,
    ///             "complexityName": "High",
    ///             "budgetEstimate": 500000.00,
    ///             "createdAt": "2024-02-15T10:00:00Z"
    ///         }
    ///     }
    /// 
    /// **Casos de uso:**
    /// - Vista detallada del proyecto
    /// - Información antes de postularse
    /// - Auditoría de proyectos
    /// </remarks>
    /// <param name="id">ID del proyecto (GUID)</param>
    /// <returns>Datos del proyecto</returns>
    /// <response code="200">Proyecto encontrado exitosamente</response>
    /// <response code="404">Proyecto no encontrado</response>
    /// <response code="401">No autenticado</response>
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
    /// <remarks>
    /// Registra un nuevo proyecto en la organización. Solo usuarios con rol Manager o Admin pueden crear proyectos.
    /// 
    /// **Ejemplo de Request:**
    /// 
    ///     POST /api/projects
    ///     {
    ///         "code": "APP-MOVIL-002",
    ///         "name": "App Móvil de Delivery",
    ///         "description": "Aplicación Android/iOS para entregas a domicilio con tracking en tiempo real",
    ///         "startDate": "2026-03-01T00:00:00Z",
    ///         "endDate": "2026-09-30T00:00:00Z",
    ///         "complexity": 1,
    ///         "budgetEstimate": 250000.00
    ///     }
    /// 
    /// **Valores de Complexity:**
    /// - 0 = Low (baja complejidad)
    /// - 1 = Medium (complejidad media)
    /// - 2 = High (alta complejidad)
    /// 
    /// **Ejemplo de Response (201 Created):**
    /// 
    ///     {
    ///         "success": true,
    ///         "message": "Proyecto creado exitosamente",
    ///         "data": "ffffffff-0000-0000-0000-000000000001"
    ///     }
    /// 
    /// **Validaciones:**
    /// - code: Requerido, único dentro de la organización
    /// - name: Requerido, máximo 200 caracteres
    /// - complexity: Rango válido 0-2
    /// - endDate: Debe ser posterior a startDate (si ambos se proporcionan)
    /// 
    /// **Casos de uso:**
    /// - Creación de nuevo proyecto
    /// - Planificación de roadmap
    /// - Presupuestación
    /// </remarks>
    /// <param name="request">Datos del proyecto (código, nombre, descripción, fechas, complejidad)</param>
    /// <returns>ID del proyecto creado</returns>
    /// <response code="201">Proyecto creado exitosamente</response>
    /// <response code="400">Datos inválidos o código duplicado</response>
    /// <response code="401">No autenticado</response>
    /// <response code="403">Sin permisos (requiere rol Manager o Admin)</response>
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
    /// <remarks>
    /// Define las habilidades técnicas requeridas para el proyecto con su nivel mínimo y si son obligatorias.
    /// 
    /// **Ejemplo de Request:**
    /// 
    ///     POST /api/projects/{projectId}/reqs
    ///     {
    ///         "skillId": "aaaaaaaa-0000-0000-0000-000000000001",
    ///         "requiredLevel": 4,
    ///         "isMandatory": true
    ///     }
    /// 
    /// **Ejemplo de Response (201 Created):**
    /// 
    ///     {
    ///         "success": true,
    ///         "message": "Requisito agregado exitosamente",
    ///         "data": "gggggggg-0000-0000-0000-000000000001"
    ///     }
    /// 
    /// **Validaciones:**
    /// - skillId: Debe existir en el catálogo
    /// - requiredLevel: Rango válido 1-5
    /// - No duplicar (skillId + projectId único)
    /// 
    /// **Parámetro isMandatory:**
    /// - true: El candidato DEBE tener esta skill (matching estricto)
    /// - false: La skill es deseable pero no bloqueante
    /// 
    /// **Casos de uso:**
    /// - Definir stack técnico del proyecto
    /// - Establecer requisitos mínimos
    /// - Base para matching de candidatos
    /// </remarks>
    /// <param name="id">ID del proyecto</param>
    /// <param name="request">Datos del requisito (skillId, nivel requerido, obligatorio)</param>
    /// <returns>ID del requisito creado</returns>
    /// <response code="201">Requisito agregado exitosamente</response>
    /// <response code="404">Proyecto no encontrado</response>
    /// <response code="400">Datos inválidos o requisito duplicado</response>
    /// <response code="401">No autenticado</response>
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
    /// Obtiene todos los requisitos de habilidades de un proyecto
    /// </summary>
    /// <remarks>
    /// Retorna el listado completo de skills requeridas para el proyecto, usado para matching de candidatos.
    /// 
    /// **Ejemplo de Request:**
    /// 
    ///     GET /api/projects/{projectId}/reqs
    ///     Authorization: Bearer {token}
    /// 
    /// **Ejemplo de Response (200 OK):**
    /// 
    ///     {
    ///         "success": true,
    ///         "message": null,
    ///         "data": [
    ///             {
    ///                 "id": "gggggggg-0000-0000-0000-000000000001",
    ///                 "projectId": "eeeeeeee-0000-0000-0000-000000000001",
    ///                 "skillId": "aaaaaaaa-0000-0000-0000-000000000001",
    ///                 "skillName": "C#",
    ///                 "requiredLevel": 5,
    ///                 "isMandatory": true
    ///             },
    ///             {
    ///                 "id": "gggggggg-0000-0000-0000-000000000002",
    ///                 "projectId": "eeeeeeee-0000-0000-0000-000000000001",
    ///                 "skillId": "aaaaaaaa-0000-0000-0000-000000000010",
    ///                 "skillName": "Docker",
    ///                 "requiredLevel": 3,
    ///                 "isMandatory": false
    ///             }
    ///         ]
    ///     }
    /// 
    /// **Casos de uso:**
    /// - Vista de requisitos antes de postularse
    /// - Comparación con skills del candidato
    /// - Input para algoritmo de matching del agente
    /// </remarks>
    /// <param name="id">ID del proyecto</param>
    /// <returns>Lista de requisitos</returns>
    /// <response code="200">Requisitos obtenidos exitosamente</response>
    /// <response code="401">No autenticado</response>
    [HttpGet("{id}/reqs")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<SkillRequirementResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSkillRequirements(Guid id)
    {
        var organizationId = Guid.Parse(User.FindFirst("OrganizationId")?.Value ?? string.Empty);

        var requirements = await _projectService.GetSkillRequirementsAsync(id, organizationId);

        return Ok(ApiResponse<IEnumerable<SkillRequirementResponse>>.SuccessResponse(requirements));
    }
}
