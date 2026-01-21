namespace API.Controllers;

using System.Security.Claims;
using Application.Common.Models;
using Application.DTOs.Profiles;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    /// Obtiene el perfil completo del usuario autenticado
    /// </summary>
    /// <remarks>
    /// Retorna la información del perfil profesional del usuario que realiza la petición.
    /// 
    /// **Ejemplo de Response (200 OK):**
    /// 
    ///     {
    ///         "success": true,
    ///         "message": null,
    ///         "data": {
    ///             "userId": "11111111-0000-0000-0000-000000000003",
    ///             "bio": "Desarrollador Full Stack con 5 años de experiencia en .NET y React",
    ///             "yearsExperience": 5,
    ///             "linkedinUrl": "https://linkedin.com/in/juan-martinez",
    ///             "githubUrl": "https://github.com/juanmartinez",
    ///             "portfolioUrl": "https://juandev.com",
    ///             "createdAt": "2024-02-01T10:00:00Z",
    ///             "updatedAt": "2025-12-15T08:30:00Z"
    ///         }
    ///     }
    /// 
    /// **Casos de uso:**
    /// - Vista de "Mi Perfil" en aplicación
    /// - Validación de completitud de perfil
    /// - Datos para CV automático
    /// 
    /// **Nota:** UserId se extrae del JWT (ClaimTypes.NameIdentifier). Si el usuario no tiene perfil, retorna 404.
    /// </remarks>
    /// <returns>Perfil del empleado autenticado</returns>
    /// <response code="200">Perfil obtenido exitosamente</response>
    /// <response code="404">Perfil no encontrado (usuario sin perfil creado)</response>
    /// <response code="401">No autenticado</response>
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
    /// Actualiza o crea el perfil del usuario autenticado
    /// </summary>
    /// <remarks>
    /// Permite al usuario actualizar su información profesional. Si el perfil no existe, lo crea automáticamente.
    /// 
    /// **Ejemplo de Request:**
    /// 
    ///     PUT /api/profile/me
    ///     {
    ///         "bio": "Senior Full Stack Developer especializado en arquitecturas cloud-native",
    ///         "yearsExperience": 8,
    ///         "linkedinUrl": "https://linkedin.com/in/juan-martinez-dev",
    ///         "githubUrl": "https://github.com/juanmartinez",
    ///         "portfolioUrl": "https://juandev.io"
    ///     }
    /// 
    /// **Ejemplo de Response (200 OK):**
    /// 
    ///     {
    ///         "success": true,
    ///         "message": "Perfil actualizado exitosamente",
    ///         "data": "Perfil actualizado exitosamente"
    ///     }
    /// 
    /// **Validaciones:**
    /// - yearsExperience: Debe estar entre 0 y 70
    /// - URLs: Formato válido (si se proporcionan)
    /// - bio: Máximo 2000 caracteres
    /// 
    /// **Comportamiento (Upsert):**
    /// - Si el perfil NO existe → Lo CREA (INSERT)
    /// - Si el perfil ya existe → Lo ACTUALIZA (UPDATE)
    /// 
    /// **Casos de uso:**
    /// - Completar perfil durante onboarding
    /// - Actualización de experiencia/proyectos
    /// - Enriquecimiento de perfil para matching
    /// </remarks>
    /// <param name="request">Datos del perfil a actualizar (bio, experiencia, URLs)</param>
    /// <returns>Resultado de la operación</returns>
    /// <response code="200">Perfil actualizado exitosamente</response>
    /// <response code="400">Datos inválidos (validación fallida)</response>
    /// <response code="401">No autenticado</response>
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
