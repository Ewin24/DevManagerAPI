namespace API.Controllers;

using API.Authorization;
using Application.Common.Models;
using Application.DTOs.Solidarity;
using Application.Interfaces;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

/// <summary>
/// Gestión de asociados (economía solidaria): alta, ciclo de vida e historial de membresía.
/// Solo administradores; todo se resuelve dentro de la organización del token.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = AppRoles.Admin)]
public class AssociatesController : ControllerBase
{
    private readonly IAssociateService _associateService;

    public AssociatesController(IAssociateService associateService)
    {
        _associateService = associateService;
    }

    /// <summary>
    /// Lista los asociados de la organización, opcionalmente filtrados por estado
    /// (1 Applicant, 2 Active, 3 Suspended, 4 Withdrawn)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<AssociateDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<AssociateDto>>>> GetAll([FromQuery] AssociateStatus? status)
    {
        var associates = await _associateService.GetAllAsync(GetOrganizationId(), status);
        return Ok(ApiResponse<IEnumerable<AssociateDto>>.SuccessResponse(associates));
    }

    /// <summary>
    /// Detalle del asociado con periodos de membresía y si tiene consentimiento de datos activo
    /// </summary>
    [HttpGet("{userId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AssociateDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<AssociateDetailDto>>> GetById(Guid userId)
    {
        var associate = await _associateService.GetByIdAsync(userId, GetOrganizationId());
        return Ok(ApiResponse<AssociateDetailDto>.SuccessResponse(associate));
    }

    /// <summary>
    /// Registra un usuario como aspirante a asociado (abre su primer periodo de membresía)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<AssociateDetailDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<AssociateDetailDto>>> Create([FromBody] CreateAssociateRequest request)
    {
        var associate = await _associateService.CreateAsync(request, GetOrganizationId(), GetCurrentUserId());

        return CreatedAtAction(
            nameof(GetById),
            new { userId = associate.UserId },
            ApiResponse<AssociateDetailDto>.SuccessResponse(associate, "Aspirante registrado exitosamente"));
    }

    /// <summary>
    /// Cambia el estado del asociado. Transiciones válidas: Applicant→Active|Withdrawn,
    /// Active→Suspended|Withdrawn, Suspended→Active|Withdrawn, Withdrawn→Applicant (reingreso).
    /// Retirar exige "reason" (400); una transición inválida responde 409.
    /// </summary>
    [HttpPost("{userId:guid}/transitions")]
    [ProducesResponseType(typeof(ApiResponse<AssociateTransitionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<AssociateTransitionResponse>>> Transition(
        Guid userId,
        [FromBody] TransitionAssociateRequest request)
    {
        var result = await _associateService.TransitionAsync(userId, request, GetOrganizationId(), GetCurrentUserId());
        return Ok(ApiResponse<AssociateTransitionResponse>.SuccessResponse(result, "Estado actualizado exitosamente"));
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
