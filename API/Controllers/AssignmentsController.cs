namespace API.Controllers;

using System.Security.Claims;
using Application.Common.Exceptions;
using Application.Common.Models;
using Application.DTOs.Assignments;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller para gestión de asignaciones a proyectos
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AssignmentsController : ControllerBase
{
    private readonly IAssignmentService _assignmentService;
    private readonly ILogger<AssignmentsController> _logger;

    public AssignmentsController(
        IAssignmentService assignmentService,
        ILogger<AssignmentsController> logger)
    {
        _assignmentService = assignmentService;
        _logger = logger;
    }

    /// <summary>
    /// Asigna un usuario a un proyecto (acción administrativa)
    /// </summary>
    /// <remarks>
    /// Permite a un manager/admin asignar formalmente a un empleado a un proyecto. Esta es la acción administrativa que confirma la participación del empleado.
    /// 
    /// **Ejemplo de Request:**
    /// 
    ///     POST /api/assignments
    ///     {
    ///         "projectId": "eeeeeeee-0000-0000-0000-000000000001",
    ///         "userId": "11111111-0000-0000-0000-000000000004",
    ///         "role": "Backend Developer",
    ///         "hoursPerWeek": 40,
    ///         "startDate": "2026-02-01T00:00:00Z",
    ///         "endDate": "2026-08-31T00:00:00Z"
    ///     }
    /// 
    /// **Ejemplo de Response (201 Created):**
    /// 
    ///     {
    ///         "success": true,
    ///         "message": "Usuario asignado exitosamente al proyecto",
    ///         "data": "iiiiiiii-0000-0000-0000-000000000001"
    ///     }
    /// 
    /// **Validaciones:**
    /// - projectId: Debe existir y pertenecer a la organización
    /// - userId: Debe existir y pertenecer a la organización
    /// - role: Requerido, máximo 100 caracteres
    /// - hoursPerWeek: Debe ser mayor a 0 y menor o igual a 168 (horas en una semana)
    /// - No puede haber asignación activa previa del mismo usuario al mismo proyecto
    /// 
    /// **Diferencia con Applications:**
    /// - Application = Postulación voluntaria del empleado (self-service)
    /// - Assignment = Asignación administrativa por manager (top-down)
    /// 
    /// **Flujo combinado:**
    /// 1. Empleado aplica: POST /api/projects/{id}/apply
    /// 2. Manager aprueba: PUT /api/applications/{id}/review (status: 1)
    /// 3. Manager asigna: POST /api/assignments
    /// 
    /// **Casos de uso:**
    /// - Asignación tras aprobar postulación
    /// - Asignación directa sin postulación previa
    /// - Reasignación de recursos entre proyectos
    /// </remarks>
    /// <param name="request">Datos de la asignación (proyecto, usuario, rol, horas, fechas)</param>
    /// <returns>ID de la asignación creada</returns>
    /// <response code="201">Usuario asignado exitosamente</response>
    /// <response code="400">El usuario ya está asignado a este proyecto</response>
    /// <response code="404">Proyecto o Usuario no encontrado</response>
    /// <response code="401">No autenticado</response>
    /// <response code="403">Sin permisos (requiere rol Manager o Admin)</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreateAssignmentRequest request)
    {
        var assignedByUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);
        var organizationId = Guid.Parse(User.FindFirst("OrganizationId")?.Value ?? string.Empty);

        try
        {
            var assignmentId = await _assignmentService.AssignUserToProjectAsync(
                request,
                organizationId,
                assignedByUserId);

            return CreatedAtAction(
                nameof(Create),
                new { id = assignmentId },
                ApiResponse<Guid>.SuccessResponse(assignmentId, "Usuario asignado exitosamente al proyecto"));
        }
        catch (NotFoundException ex)
        {
            return NotFound();
        }
        catch (BusinessValidationException ex)
        {
            return BadRequest();
        }
    }
}
