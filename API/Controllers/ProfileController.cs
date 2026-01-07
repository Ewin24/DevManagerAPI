namespace API.Controllers;

using Application.Common.Models;
using Application.DTOs.Profiles;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

/// <summary>
/// Controller para gestión de perfiles de empleados
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly IProfileService _profileService;
    private readonly ILogger<ProfileController> _logger;

    public ProfileController(IProfileService profileService, ILogger<ProfileController> logger)
    {
        _profileService = profileService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene el perfil del usuario autenticado
    /// </summary>
    /// <returns>Perfil del empleado</returns>
    [HttpGet("me")]
    [ProducesResponseType(typeof(ApiResponse<EmployeeProfileDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyProfile()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);
        var organizationId = Guid.Parse(User.FindFirst("OrganizationId")?.Value ?? string.Empty);

        var profile = await _profileService.GetMyProfileAsync(userId, organizationId);

        if (profile == null)
        {
            return NotFound();
        }

        return Ok(ApiResponse<EmployeeProfileDto>.SuccessResponse(profile));
    }

    /// <summary>
    /// Actualiza el perfil del usuario autenticado
    /// </summary>
    /// <param name="request">Datos del perfil a actualizar</param>
    /// <returns>Resultado de la operación</returns>
    [HttpPut("me")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateProfileRequest request)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty);
        var organizationId = Guid.Parse(User.FindFirst("OrganizationId")?.Value ?? string.Empty);

        var result = await _profileService.UpdateMyProfileAsync(userId, organizationId, request);

        if (!result)
        {
            return BadRequest();
        }

        return Ok(ApiResponse<object>.SuccessResponse("Perfil actualizado exitosamente"));
    }
}
