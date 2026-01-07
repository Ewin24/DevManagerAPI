namespace API.Controllers;

using Application.Common.Exceptions;
using Application.Common.Models;
using Application.DTOs.Applications;
using Application.Interfaces;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
    /// Aplica a un proyecto
    /// </summary>
    /// <param name="id">ID del proyecto</param>
    /// <param name="request">Mensaje de aplicación</param>
    /// <returns>ID de la aplicación creada</returns>
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
    /// Revisa una postulación (aprobar/rechazar)
    /// </summary>
    /// <param name="id">ID de la postulación</param>
    /// <param name="request">Datos de la revisión</param>
    /// <returns>Resultado de la operación</returns>
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
    /// <param name="id">ID del proyecto</param>
    /// <returns>Lista de postulaciones</returns>
    [HttpGet("projects/{id}/applications")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ApplicationResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProjectApplications(Guid id)
    {
        var organizationId = Guid.Parse(User.FindFirst("OrganizationId")?.Value ?? string.Empty);

        var applications = await _applicationService.GetProjectApplicationsAsync(id, organizationId);

        return Ok(ApiResponse<IEnumerable<ApplicationResponse>>.SuccessResponse(applications));
    }
}
