namespace API.Controllers;

using API.Authorization;
using Application.Common.Models;
using Application.DTOs.Solidarity;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

/// <summary>
/// Habeas Data: políticas de tratamiento de datos versionadas y consentimientos auditados.
/// Escrituras y listados solo para administradores; la política vigente la puede leer cualquier usuario autenticado.
/// </summary>
[ApiController]
[Authorize]
public class ConsentsController : ControllerBase
{
    private readonly IConsentService _consentService;

    public ConsentsController(IConsentService consentService)
    {
        _consentService = consentService;
    }

    /// <summary>Lista las versiones de política de datos de la organización (más reciente primero)</summary>
    [Authorize(Roles = AppRoles.Admin)]
    [HttpGet("api/data-policies")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<DataPolicyDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<DataPolicyDto>>>> GetPolicies()
    {
        var policies = await _consentService.GetPoliciesAsync(GetOrganizationId());
        return Ok(ApiResponse<IEnumerable<DataPolicyDto>>.SuccessResponse(policies));
    }

    /// <summary>Política vigente (la de mayor fecha de vigencia). 404 si la organización aún no publicó ninguna</summary>
    [HttpGet("api/data-policies/current")]
    [ProducesResponseType(typeof(ApiResponse<DataPolicyDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<DataPolicyDto>>> GetCurrentPolicy()
    {
        var policy = await _consentService.GetCurrentPolicyAsync(GetOrganizationId());
        if (policy == null)
        {
            throw new Application.Common.Exceptions.NotFoundException("La organización no tiene una política de datos publicada");
        }
        return Ok(ApiResponse<DataPolicyDto>.SuccessResponse(policy));
    }

    /// <summary>Publica una nueva versión de política. El texto publicado nunca cambia</summary>
    [Authorize(Roles = AppRoles.Admin)]
    [HttpPost("api/data-policies")]
    [ProducesResponseType(typeof(ApiResponse<DataPolicyDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<DataPolicyDto>>> CreatePolicy([FromBody] CreateDataPolicyRequest request)
    {
        var policy = await _consentService.CreatePolicyAsync(request, GetOrganizationId(), GetCurrentUserId());
        return CreatedAtAction(
            nameof(GetPolicies),
            ApiResponse<DataPolicyDto>.SuccessResponse(policy, "Política publicada exitosamente"));
    }

    /// <summary>Editar una política publicada siempre se rechaza (409): se requiere una nueva versión</summary>
    [Authorize(Roles = AppRoles.Admin)]
    [HttpPut("api/data-policies/{id:guid}")]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdatePolicy(Guid id)
    {
        await _consentService.UpdatePolicyAsync(id, GetOrganizationId());
        return NoContent();
    }

    /// <summary>Historial completo de consentimientos del usuario (incluye revocados)</summary>
    [Authorize(Roles = AppRoles.Admin)]
    [HttpGet("api/users/{userId:guid}/consents")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ConsentDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<IEnumerable<ConsentDto>>>> GetUserConsents(Guid userId)
    {
        var consents = await _consentService.GetUserConsentsAsync(userId, GetOrganizationId());
        return Ok(ApiResponse<IEnumerable<ConsentDto>>.SuccessResponse(consents));
    }

    /// <summary>Registra el consentimiento del usuario para una versión de política</summary>
    [Authorize(Roles = AppRoles.Admin)]
    [HttpPost("api/users/{userId:guid}/consents")]
    [ProducesResponseType(typeof(ApiResponse<ConsentDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<ConsentDto>>> Grant(Guid userId, [FromBody] GrantConsentRequest request)
    {
        var consent = await _consentService.GrantAsync(userId, request, GetOrganizationId(), GetCurrentUserId());
        return CreatedAtAction(
            nameof(GetUserConsents),
            new { userId },
            ApiResponse<ConsentDto>.SuccessResponse(consent, "Consentimiento registrado exitosamente"));
    }

    /// <summary>Revoca un consentimiento (motivo obligatorio). El registro se conserva en el historial</summary>
    [Authorize(Roles = AppRoles.Admin)]
    [HttpPost("api/consents/{id:guid}/revoke")]
    [ProducesResponseType(typeof(ApiResponse<ConsentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApiResponse<ConsentDto>>> Revoke(Guid id, [FromBody] RevokeConsentRequest request)
    {
        var consent = await _consentService.RevokeAsync(id, request, GetOrganizationId(), GetCurrentUserId());
        return Ok(ApiResponse<ConsentDto>.SuccessResponse(consent, "Consentimiento revocado exitosamente"));
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
