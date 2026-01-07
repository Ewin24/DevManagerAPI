namespace API.Controllers;

using System.Security.Claims;
using Application.Common.Models;
using Application.DTOs.Users;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controlador de gestión de usuarios
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        IUserService userService,
        ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todos los usuarios de la organización
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<UserResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<UserResponse>>>> GetAll()
    {
        var organizationId = GetOrganizationId();
        var users = await _userService.GetAllAsync(organizationId);
        return Ok(ApiResponse<IEnumerable<UserResponse>>.SuccessResponse(users));
    }

    /// <summary>
    /// Obtiene un usuario por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<UserResponse>>> GetById(Guid id)
    {
        var organizationId = GetOrganizationId();
        var user = await _userService.GetByIdAsync(id, organizationId);
        return Ok(ApiResponse<UserResponse>.SuccessResponse(user));
    }

    /// <summary>
    /// Crea un nuevo usuario
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<UserResponse>>> Create(
        [FromBody] CreateUserRequest request)
    {
        var organizationId = GetOrganizationId();
        var currentUserId = GetCurrentUserId();

        var user = await _userService.CreateAsync(request, organizationId, currentUserId);

        return CreatedAtAction(
            nameof(GetById),
            new { id = user.Id },
            ApiResponse<UserResponse>.SuccessResponse(user, "Usuario creado exitosamente"));
    }

    /// <summary>
    /// Actualiza un usuario existente
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<UserResponse>>> Update(
        Guid id,
        [FromBody] UpdateUserRequest request)
    {
        var organizationId = GetOrganizationId();
        var currentUserId = GetCurrentUserId();

        var user = await _userService.UpdateAsync(id, request, organizationId, currentUserId);

        return Ok(ApiResponse<UserResponse>.SuccessResponse(user, "Usuario actualizado exitosamente"));
    }

    /// <summary>
    /// Elimina lógicamente un usuario
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var organizationId = GetOrganizationId();
        var currentUserId = GetCurrentUserId();

        await _userService.DeleteAsync(id, organizationId, currentUserId);

        return NoContent();
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
