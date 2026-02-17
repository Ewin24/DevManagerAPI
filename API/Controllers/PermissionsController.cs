namespace API.Controllers;

using Application.Common.Exceptions;
using Application.Common.Models;
using Application.DTOs.RolesPermissions;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

/// <summary>
/// Controller para gestión de permisos RBAC y validación
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PermissionsController : ControllerBase
{
    private readonly IRolePermissionService _service;
    private readonly ILogger<PermissionsController> _logger;

    public PermissionsController(IRolePermissionService service, ILogger<PermissionsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    private Guid GetOrganizationId() =>
        Guid.Parse(User.FindFirst("OrganizationId")?.Value ?? string.Empty);

    // ==================== PERMISSIONS CRUD ====================

    /// <summary>
    /// Obtiene todos los permisos del sistema agrupados por módulo
    /// </summary>
    [HttpGet("grouped")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<PermissionsByModuleResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllGrouped()
    {
        var permissions = await _service.GetAllPermissionsGroupedAsync();
        return Ok(ApiResponse<IEnumerable<PermissionsByModuleResponse>>.SuccessResponse(
            permissions, "Permisos obtenidos exitosamente"));
    }

    /// <summary>
    /// Obtiene todos los permisos del sistema como lista plana
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<PermissionResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var permissions = await _service.GetAllPermissionsAsync();
        return Ok(ApiResponse<IEnumerable<PermissionResponse>>.SuccessResponse(
            permissions, "Permisos obtenidos exitosamente"));
    }

    /// <summary>
    /// Obtiene un permiso por su ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<PermissionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var permission = await _service.GetPermissionByIdAsync(id);
            return Ok(ApiResponse<PermissionResponse>.SuccessResponse(permission, "Permiso obtenido exitosamente"));
        }
        catch (NotFoundException ex)
        {
            return NotFound(new ErrorResponse { Message = ex.Message });
        }
    }

    /// <summary>
    /// Crea un nuevo permiso en el sistema
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<PermissionResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreatePermissionRequest request)
    {
        try
        {
            var permission = await _service.CreatePermissionAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = permission.Id },
                ApiResponse<PermissionResponse>.SuccessResponse(permission, "Permiso creado exitosamente"));
        }
        catch (ConflictException ex)
        {
            return Conflict(new ErrorResponse { Message = ex.Message });
        }
    }

    /// <summary>
    /// Actualiza un permiso existente
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<PermissionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePermissionRequest request)
    {
        try
        {
            var permission = await _service.UpdatePermissionAsync(id, request);
            return Ok(ApiResponse<PermissionResponse>.SuccessResponse(permission, "Permiso actualizado exitosamente"));
        }
        catch (NotFoundException ex)
        {
            return NotFound(new ErrorResponse { Message = ex.Message });
        }
    }

    /// <summary>
    /// Elimina un permiso (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _service.DeletePermissionAsync(id);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new ErrorResponse { Message = ex.Message });
        }
    }

    // ==================== USER PERMISSION OVERRIDES ====================

    /// <summary>
    /// Asigna un permiso directo a un usuario (override sobre permisos del rol)
    /// </summary>
    /// <remarks>
    /// Permite asignar (IsGranted=true) o denegar (IsGranted=false) un permiso 
    /// directamente a un usuario, independiente de sus roles.
    /// Los permisos denegados (IsGranted=false) tienen prioridad sobre los permisos del rol.
    /// </remarks>
    [HttpPost("assign-to-user")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignToUser([FromBody] AssignPermissionToUserRequest request)
    {
        var orgId = GetOrganizationId();
        try
        {
            await _service.AssignPermissionToUserAsync(request, orgId);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new ErrorResponse { Message = ex.Message });
        }
    }

    /// <summary>
    /// Revoca un permiso directo de un usuario
    /// </summary>
    [HttpDelete("revoke-from-user/{userId}/{permissionId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RevokeFromUser(Guid userId, Guid permissionId)
    {
        var orgId = GetOrganizationId();
        await _service.RevokePermissionFromUserAsync(userId, permissionId, orgId);
        return NoContent();
    }

    // ==================== EFFECTIVE PERMISSIONS & VALIDATION ====================

    /// <summary>
    /// Obtiene los permisos efectivos de un usuario (roles + overrides directos)
    /// </summary>
    [HttpGet("user/{userId}/effective")]
    [ProducesResponseType(typeof(ApiResponse<UserEffectivePermissionsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEffectivePermissions(Guid userId)
    {
        var orgId = GetOrganizationId();
        try
        {
            var result = await _service.GetUserEffectivePermissionsAsync(userId, orgId);
            return Ok(ApiResponse<UserEffectivePermissionsResponse>.SuccessResponse(
                result, "Permisos efectivos obtenidos exitosamente"));
        }
        catch (NotFoundException ex)
        {
            return NotFound(new ErrorResponse { Message = ex.Message });
        }
    }

    /// <summary>
    /// Valida si un usuario tiene un permiso específico
    /// </summary>
    /// <remarks>
    /// Verifica si el usuario tiene el permiso dado, considerando:
    /// 1. Denegaciones explícitas (UserPermissions con IsGranted=false) - prioridad máxima
    /// 2. Otorgamientos directos (UserPermissions con IsGranted=true)
    /// 3. Permisos de roles (RolePermissions a través de UserRoles)
    /// </remarks>
    [HttpPost("validate")]
    [ProducesResponseType(typeof(ApiResponse<ValidatePermissionResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Validate([FromBody] ValidatePermissionRequest request)
    {
        var orgId = GetOrganizationId();
        var result = await _service.ValidatePermissionAsync(request, orgId);
        return Ok(ApiResponse<ValidatePermissionResponse>.SuccessResponse(
            result, "Validación completada"));
    }
}