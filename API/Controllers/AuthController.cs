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
    /// Autenticación de usuario en el sistema
    /// </summary>
    /// <remarks>
    /// Valida las credenciales del usuario y retorna un token JWT válido por 7 días.
    /// 
    /// **Ejemplo de Request:**
    /// 
    ///     POST /api/auth/login
    ///     {
    ///         "email": "admin@techcorp.com",
    ///         "password": "Password123!"
    ///     }
    /// 
    /// **Ejemplo de Response (200 OK):**
    /// 
    ///     {
    ///         "success": true,
    ///         "message": "Login exitoso",
    ///         "data": {
    ///             "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    ///             "userId": "11111111-0000-0000-0000-000000000001",
    ///             "email": "admin@techcorp.com",
    ///             "fullName": "Admin TechCorp",
    ///             "organizationId": "11111111-1111-1111-1111-111111111111",
    ///             "organizationName": "TechCorp",
    ///             "role": "Admin"
    ///         }
    ///     }
    /// 
    /// **Token JWT incluye:**
    /// - UserId (claim: nameid)
    /// - Email (claim: email)
    /// - OrganizationId (claim: OrganizationId)
    /// - Role (claim: role)
    /// - Expiración: 7 días desde emisión
    /// 
    /// **Seguridad:**
    /// - Utiliza HMACSHA512 para hashing de contraseñas
    /// - Token debe incluirse en header: Authorization: Bearer {token}
    /// </remarks>
    /// <param name="request">Credenciales de acceso (email y password)</param>
    /// <response code="200">Login exitoso - Retorna token JWT y datos del usuario</response>
    /// <response code="401">Credenciales inválidas - Email o contraseña incorrectos</response>
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
    /// <remarks>
    /// Crea una nueva organización en el sistema y automáticamente:
    /// - Genera 3 roles por defecto: Admin, Manager, Developer
    /// - Crea el usuario administrador principal
    /// - Retorna token JWT para acceso inmediato
    /// 
    /// **Ejemplo de Request:**
    /// 
    ///     POST /api/auth/register-organization
    ///     {
    ///         "organizationName": "InnovateLab",
    ///         "legalName": "InnovateLab S.A. de C.V.",
    ///         "adminEmail": "admin@innovatelab.com",
    ///         "adminPassword": "SecurePass123!",
    ///         "adminFullName": "Carlos Ruiz"
    ///     }
    /// 
    /// **Ejemplo de Response (201 Created):**
    /// 
    ///     {
    ///         "success": true,
    ///         "message": "Organización registrada exitosamente",
    ///         "data": {
    ///             "organizationId": "22222222-2222-2222-2222-222222222222",
    ///             "organizationName": "InnovateLab",
    ///             "adminUserId": "22222222-0000-0000-0000-000000000001",
    ///             "adminEmail": "admin@innovatelab.com",
    ///             "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
    ///         }
    ///     }
    /// 
    /// **Validaciones:**
    /// - Email único a nivel global
    /// - Contraseña mínimo 8 caracteres
    /// - OrganizationName único
    /// 
    /// **Casos de uso:**
    /// - Onboarding de nuevas organizaciones
    /// - Creación de cuentas trial
    /// - Registro self-service
    /// </remarks>
    /// <param name="request">Datos de la organización y administrador inicial</param>
    /// <response code="201">Organización creada exitosamente con usuario admin</response>
    /// <response code="400">Datos inválidos o email/organización duplicados</response>
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
