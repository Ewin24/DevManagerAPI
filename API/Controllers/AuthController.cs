namespace API.Controllers;

using Application.Common.Models;
using Application.DTOs.Auth;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controlador de autenticación y registro
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IAuthService authService,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Login de usuario
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login(
        [FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        return Ok(ApiResponse<LoginResponse>.SuccessResponse(result, "Login exitoso"));
    }

    /// <summary>
    /// Registro de nueva organización con usuario administrador
    /// </summary>
    [HttpPost("register-organization")]
    [ProducesResponseType(typeof(ApiResponse<RegisterOrganizationResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<RegisterOrganizationResponse>>> RegisterOrganization(
        [FromBody] RegisterOrganizationRequest request)
    {
        var result = await _authService.RegisterOrganizationAsync(request);
        return CreatedAtAction(
            nameof(RegisterOrganization),
            ApiResponse<RegisterOrganizationResponse>.SuccessResponse(
                result,
                "Organización registrada exitosamente"));
    }
}
