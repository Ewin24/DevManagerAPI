namespace API.Controllers;

using Application.Common.Exceptions;
using Application.Common.Models;
using Application.DTOs.Assignments;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
    /// Asigna un usuario a un proyecto
    /// </summary>
    /// <param name="request">Datos de la asignación</param>
    /// <returns>ID de la asignación creada</returns>
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
