namespace API.Controllers;

using API.Authorization;
using Application.Common.Models;
using Application.DTOs.Solidarity;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

/// <summary>
/// Educación cooperativa: programas e inscripciones con horas completadas.
/// Solo administradores; todo se resuelve dentro de la organización del token.
/// </summary>
[ApiController]
[Route("api/education")]
[Authorize(Roles = AppRoles.Admin)]
public class EducationController : ControllerBase
{
    private readonly IEducationService _educationService;

    public EducationController(IEducationService educationService)
    {
        _educationService = educationService;
    }

    /// <summary>Lista los programas de la organización</summary>
    [HttpGet("programs")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<EducationProgramDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<EducationProgramDto>>>> GetPrograms()
    {
        var programs = await _educationService.GetProgramsAsync(GetOrganizationId());
        return Ok(ApiResponse<IEnumerable<EducationProgramDto>>.SuccessResponse(programs));
    }

    /// <summary>Obtiene un programa por ID</summary>
    [HttpGet("programs/{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<EducationProgramDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<EducationProgramDto>>> GetProgram(Guid id)
    {
        var program = await _educationService.GetProgramByIdAsync(id, GetOrganizationId());
        return Ok(ApiResponse<EducationProgramDto>.SuccessResponse(program));
    }

    /// <summary>
    /// Crea un programa. Tipo: 1 Basic, 2 Permanent, 3 Governance, 4 Informal. Horas requeridas &gt; 0 (400 si no).
    /// </summary>
    [HttpPost("programs")]
    [ProducesResponseType(typeof(ApiResponse<EducationProgramDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<EducationProgramDto>>> CreateProgram([FromBody] SaveEducationProgramRequest request)
    {
        var program = await _educationService.CreateProgramAsync(request, GetOrganizationId(), GetCurrentUserId());

        return CreatedAtAction(
            nameof(GetProgram),
            new { id = program.Id },
            ApiResponse<EducationProgramDto>.SuccessResponse(program, "Programa creado exitosamente"));
    }

    /// <summary>Actualiza un programa</summary>
    [HttpPut("programs/{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<EducationProgramDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<EducationProgramDto>>> UpdateProgram(Guid id, [FromBody] SaveEducationProgramRequest request)
    {
        var program = await _educationService.UpdateProgramAsync(id, request, GetOrganizationId(), GetCurrentUserId());
        return Ok(ApiResponse<EducationProgramDto>.SuccessResponse(program, "Programa actualizado exitosamente"));
    }

    /// <summary>Elimina (borrado lógico) un programa</summary>
    [HttpDelete("programs/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProgram(Guid id)
    {
        await _educationService.DeleteProgramAsync(id, GetOrganizationId(), GetCurrentUserId());
        return NoContent();
    }

    /// <summary>Lista las inscripciones de un programa</summary>
    [HttpGet("programs/{programId:guid}/enrollments")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<EnrollmentDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<IEnumerable<EnrollmentDto>>>> GetEnrollments(Guid programId)
    {
        var enrollments = await _educationService.GetEnrollmentsAsync(programId, GetOrganizationId());
        return Ok(ApiResponse<IEnumerable<EnrollmentDto>>.SuccessResponse(enrollments));
    }

    /// <summary>
    /// Inscribe a un asociado en el programa. 409 si el asociado está retirado o ya está inscrito;
    /// 400 si el estado es Completed (2) sin fecha de finalización.
    /// </summary>
    [HttpPost("programs/{programId:guid}/enrollments")]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<EnrollmentDto>>> Enroll(Guid programId, [FromBody] CreateEnrollmentRequest request)
    {
        var enrollment = await _educationService.EnrollAsync(programId, request, GetOrganizationId(), GetCurrentUserId());

        return CreatedAtAction(
            nameof(GetEnrollments),
            new { programId },
            ApiResponse<EnrollmentDto>.SuccessResponse(enrollment, "Inscripción creada exitosamente"));
    }

    /// <summary>
    /// Actualiza estado y horas de una inscripción (1 Enrolled, 2 Completed, 3 Dropped).
    /// Completed exige fecha de finalización (400).
    /// </summary>
    [HttpPut("enrollments/{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<EnrollmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<EnrollmentDto>>> UpdateEnrollment(Guid id, [FromBody] UpdateEnrollmentRequest request)
    {
        var enrollment = await _educationService.UpdateEnrollmentAsync(id, request, GetOrganizationId(), GetCurrentUserId());
        return Ok(ApiResponse<EnrollmentDto>.SuccessResponse(enrollment, "Inscripción actualizada exitosamente"));
    }

    #region Helper Methods

    private Guid GetOrganizationId()
    {
        var orgIdClaim = User.FindFirstValue("OrganizationId");
        if (string.IsNullOrEmpty(orgIdClaim) || !Guid.TryParse(orgIdClaim, out var orgId))
        {
            throw new UnauthorizedAccessException("OrganizationId no encontrado en el token");
        }
        return orgId;
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("UserId no encontrado en el token");
        }
        return userId;
    }

    #endregion
}
