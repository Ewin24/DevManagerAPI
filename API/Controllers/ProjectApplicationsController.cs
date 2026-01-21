namespace API.Controllers;

using System.Security.Claims;
using Application.Common.Exceptions;
using Application.Common.Models;
using Application.DTOs.Applications;
using Application.Interfaces;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller para gestión de postulaciones a proyectos
/// </summary>
[ApiController]
[Route("api")]
[Authorize]
public class ProjectApplicationsController : ControllerBase
{
    private readonly IApplicationService _applicationService;
    private readonly ILogger<ProjectApplicationsController> _logger;

    public ProjectApplicationsController(
        IApplicationService applicationService,
        ILogger<ProjectApplicationsController> logger)
    {
        _applicationService = applicationService;
        _logger = logger;
    }

    /// <summary>
    /// Aplica/postula a un proyecto (self-service)
    /// </summary>
    /// <remarks>
    /// Permite a un empleado postularse voluntariamente a un proyecto disponible.
    /// 
    /// **Ejemplo de Request:**
    /// 
    ///     POST /api/projects/{projectId}/apply
    ///     {
    ///         "message": "Tengo 5 años de experiencia en .NET y Azure, he trabajado en proyectos similares de sistemas hospitalarios. Me gustaría contribuir en el backend y la integración cloud."
    ///     }
    /// 
    /// **Ejemplo de Response (201 Created):**
    /// 
    ///     {
    ///         "success": true,
    ///         "message": "Postulación enviada exitosamente",
    ///         "data": "hhhhhhhh-0000-0000-0000-000000000001"
    ///     }
    /// 
    /// **Validaciones:**
    /// - El proyecto debe existir y estar activo
    /// - No puede haber postulación previa del mismo usuario al mismo proyecto
    /// - El usuario debe pertenecer a la misma organización
    /// 
    /// **Casos de uso:**
    /// - Empleado aplica a proyecto disponible
    /// - Auto-asignación a proyectos abiertos
    /// - Manifestación de interés
    /// 
    /// **Comportamiento:**
    /// - Estado inicial: Pending (0)
    /// - UserId se toma del JWT automáticamente
    /// - AppliedAt se establece automáticamente
    /// </remarks>
    /// <param name="id">ID del proyecto</param>
    /// <param name="request">Mensaje opcional de presentación</param>
    /// <returns>ID de la aplicación creada</returns>
    /// <response code="201">Postulación enviada exitosamente</response>
    /// <response code="400">Ya existe una postulación activa para este proyecto</response>
    /// <response code="404">Proyecto no encontrado</response>
    /// <response code="401">No autenticado</response>
    [HttpPost("projects/{id}/apply")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Apply(Guid id, [FromBody] ApplyToProjectRequest request)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);
        var organizationId = Guid.Parse(User.FindFirst("OrganizationId")?.Value ?? string.Empty);

        try
        {
            var applicationId = await _applicationService.ApplyToProjectAsync(
                id,
                userId,
                organizationId,
                request.Message);

            return CreatedAtAction(
                nameof(Apply),
                new { id = applicationId },
                ApiResponse<Guid>.SuccessResponse(applicationId, "Postulación enviada exitosamente"));
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

    /// <summary>
    /// Revisa una postulación (aprobar o rechazar) - Solo managers/admins
    /// </summary>
    /// <remarks>
    /// Permite a un manager/admin revisar y decidir sobre una postulación de empleado.
    /// 
    /// **Valores de Status:**
    /// - 1 = Approved (aprobada)
    /// - 2 = Rejected (rechazada)
    /// 
    /// **Ejemplo de Request - Aprobar:**
    /// 
    ///     PUT /api/applications/{applicationId}/review
    ///     {
    ///         "status": 1,
    ///         "reviewNotes": "Perfil excelente, cumple todos los requisitos técnicos"
    ///     }
    /// 
    /// **Ejemplo de Request - Rechazar:**
    /// 
    ///     PUT /api/applications/{applicationId}/review
    ///     {
    ///         "status": 2,
    ///         "reviewNotes": "No cumple con el nivel requerido de Azure (necesitamos nivel 4+)"
    ///     }
    /// 
    /// **Ejemplo de Response (200 OK):**
    /// 
    ///     {
    ///         "success": true,
    ///         "message": "Postulación aprobada exitosamente",
    ///         "data": "Postulación aprobada exitosamente"
    ///     }
    /// 
    /// **Comportamiento:**
    /// - Establece ReviewedByUserId del JWT
    /// - Establece ReviewedAt = SYSUTCDATETIME()
    /// - Actualiza Status a Approved (1) o Rejected (2)
    /// - Solo se puede revisar postulaciones en estado Pending (0)
    /// 
    /// **Flujo recomendado:**
    /// 1. Empleado aplica: POST /api/projects/{id}/apply
    /// 2. Manager consulta: GET /api/projects/{id}/applications
    /// 3. Manager revisa: PUT /api/applications/{id}/review
    /// 4. Si aprueba → Crear assignment: POST /api/assignments
    /// 
    /// **Casos de uso:**
    /// - Manager revisa postulaciones del proyecto
    /// - Proceso de selección de candidatos
    /// - Feedback a empleados sobre postulación
    /// </remarks>
    /// <param name="id">ID de la postulación (ApplicationId)</param>
    /// <param name="request">Decisión (status: 1=Aprobada, 2=Rechazada) y notas de revisión</param>
    /// <returns>Resultado de la operación</returns>
    /// <response code="200">Postulación revisada exitosamente</response>
    /// <response code="400">La postulación ya fue revisada</response>
    /// <response code="404">Postulación no encontrada</response>
    /// <response code="401">No autenticado</response>
    /// <response code="403">Sin permisos (requiere rol Manager o Admin)</response>
    [HttpPut("applications/{id}/review")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Review(Guid id, [FromBody] ReviewApplicationRequest request)
    {
        var reviewedByUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);
        var organizationId = Guid.Parse(User.FindFirst("OrganizationId")?.Value ?? string.Empty);

        try
        {
            var result = await _applicationService.ReviewApplicationAsync(
                id,
                organizationId,
                request.Status,
                reviewedByUserId,
                request.ReviewNotes);

            if (!result)
            {
                return BadRequest();
            }

            var statusText = request.Status == ApplicationStatus.Approved ? "aprobada" : "rechazada";
            return Ok(ApiResponse<object>.SuccessResponse($"Postulación {statusText} exitosamente"));
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

    /// <summary>
    /// Obtiene todas las postulaciones de un proyecto
    /// </summary>
    /// <remarks>
    /// Retorna el listado completo de empleados que se han postulado al proyecto con el estado de su postulación.
    /// 
    /// **Ejemplo de Request:**
    /// 
    ///     GET /api/projects/{projectId}/applications
    ///     Authorization: Bearer {token}
    /// 
    /// **Ejemplo de Response (200 OK):**
    /// 
    ///     {
    ///         "success": true,
    ///         "message": null,
    ///         "data": [
    ///             {
    ///                 "id": "hhhhhhhh-0000-0000-0000-000000000001",
    ///                 "projectId": "eeeeeeee-0000-0000-0000-000000000001",
    ///                 "projectName": "Sistema de Gestión Hospitalaria",
    ///                 "userId": "11111111-0000-0000-0000-000000000003",
    ///                 "userFullName": "Juan Martínez",
    ///                 "message": "Tengo 5 años de experiencia en .NET y Azure...",
    ///                 "status": 0,
    ///                 "statusName": "Pending",
    ///                 "appliedAt": "2026-01-15T10:30:00Z",
    ///                 "reviewedByUserId": null,
    ///                 "reviewedByName": null,
    ///                 "reviewedAt": null,
    ///                 "reviewNotes": null
    ///             },
    ///             {
    ///                 "id": "hhhhhhhh-0000-0000-0000-000000000002",
    ///                 "projectId": "eeeeeeee-0000-0000-0000-000000000001",
    ///                 "projectName": "Sistema de Gestión Hospitalaria",
    ///                 "userId": "11111111-0000-0000-0000-000000000004",
    ///                 "userFullName": "Ana López",
    ///                 "message": "Backend developer con experiencia en microservicios...",
    ///                 "status": 1,
    ///                 "statusName": "Approved",
    ///                 "appliedAt": "2026-01-14T14:00:00Z",
    ///                 "reviewedByUserId": "11111111-0000-0000-0000-000000000002",
    ///                 "reviewedByName": "María García",
    ///                 "reviewedAt": "2026-01-16T09:15:00Z",
    ///                 "reviewNotes": "Excelente perfil técnico"
    ///             }
    ///         ]
    ///     }
    /// 
    /// **Estados de Postulación:**
    /// - 0 = Pending (pendiente de revisión)
    /// - 1 = Approved (aprobada)
    /// - 2 = Rejected (rechazada)
    /// 
    /// **Casos de uso:**
    /// - Manager revisa candidatos interesados
    /// - Dashboard de postulaciones del proyecto
    /// - Comparación de perfiles
    /// </remarks>
    /// <param name="id">ID del proyecto</param>
    /// <returns>Lista de postulaciones</returns>
    /// <response code="200">Postulaciones obtenidas exitosamente</response>
    /// <response code="401">No autenticado</response>
    [HttpGet("projects/{id}/applications")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ApplicationResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProjectApplications(Guid id)
    {
        var organizationId = Guid.Parse(User.FindFirst("OrganizationId")?.Value ?? string.Empty);

        var applications = await _applicationService.GetProjectApplicationsAsync(id, organizationId);

        return Ok(ApiResponse<IEnumerable<ApplicationResponse>>.SuccessResponse(applications));
    }
}
