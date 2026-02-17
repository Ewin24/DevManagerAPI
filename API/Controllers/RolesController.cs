namespace API.Controllers;

using Application.Common.Exceptions;
using Application.Common.Models;
using Application.DTOs.RolesPermissions;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

/// <summary>
/// Controller para gestión de roles RBAC
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly IRolePermissionService _service;
    private readonly ILogger<RolesController> _logger;

    public RolesController(IRolePermissionService service, ILogger<RolesController> logger)
    {
        _service = service;
        _logger = logger;
    }

    private Guid GetOrganizationId() =>
        Guid.Parse(User.FindFirst("OrganizationId")?.Value ?? string.Empty);

    /// <summary>
    /// Obtiene todos los roles de la organización
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<RoleSummaryResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var orgId = GetOrganizationId();
        var roles = await _service.GetAllRolesAsync(orgId);
        return Ok(ApiResponse<IEnumerable<RoleSummaryResponse>>.SuccessResponse(roles, "Roles obtenidos exitosamente"));
    }

    /// <summary>
    /// Obtiene un rol por su ID con sus permisos asignados
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<RoleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var orgId = GetOrganizationId();
        try
        {
            var role = await _service.GetRoleByIdAsync(id, orgId);
            return Ok(ApiResponse<RoleResponse>.SuccessResponse(role, "Rol obtenido exitosamente"));
        }
        catch (NotFoundException ex)
        {
            return NotFound(new ErrorResponse { Message = ex.Message });
        }
    }

    /// <summary>
    /// Crea un nuevo rol en la organización
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<RoleResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateRoleRequest request)
    {
        var orgId = GetOrganizationId();
        try
        {
            var role = await _service.CreateRoleAsync(request, orgId);
            return CreatedAtAction(nameof(GetById), new { id = role.Id },
                ApiResponse<RoleResponse>.SuccessResponse(role, "Rol creado exitosamente"));
        }
        catch (ConflictException ex)
        {
            return Conflict(new ErrorResponse { Message = ex.Message });
        }
    }

    /// <summary>
    /// Actualiza un rol existente
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<RoleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRoleRequest request)
    {
        var orgId = GetOrganizationId();
        try
        {
            var role = await _service.UpdateRoleAsync(id, request, orgId);
            return Ok(ApiResponse<RoleResponse>.SuccessResponse(role, "Rol actualizado exitosamente"));
        }
        catch (NotFoundException ex)
        {
            return NotFound(new ErrorResponse { Message = ex.Message });
        }
        catch (ConflictException ex)
        {
            return Conflict(new ErrorResponse { Message = ex.Message });
        }
    }

    /// <summary>
    /// Elimina un rol (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var orgId = GetOrganizationId();
        try
        {
            await _service.DeleteRoleAsync(id, orgId);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new ErrorResponse { Message = ex.Message });
        }
        catch (BusinessValidationException ex)
        {
            return BadRequest(new ErrorResponse { Message = ex.Message });
        }
    }

    // ==================== PERMISOS DEL ROL ====================

    /// <summary>
    /// Obtiene los permisos asignados a un rol
    /// </summary>
    [HttpGet("{id}/permissions")]
    [ProducesResponseType(typeof(ApiResponse<RoleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRolePermissions(Guid id)
    {
        var orgId = GetOrganizationId();
        try
        {
            var role = await _service.GetRoleWithPermissionsAsync(id, orgId);
            return Ok(ApiResponse<RoleResponse>.SuccessResponse(role, "Permisos del rol obtenidos"));
        }
        catch (NotFoundException ex)
        {
            return NotFound(new ErrorResponse { Message = ex.Message });
        }
    }

    /// <summary>
    /// Asigna permisos a un rol (reemplaza los permisos actuales)
    /// </summary>
    [HttpPut("{id}/permissions")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignPermissions(Guid id, [FromBody] AssignPermissionsToRoleRequest request)
    {
        var orgId = GetOrganizationId();
        try
        {
            await _service.AssignPermissionsToRoleAsync(id, request, orgId);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new ErrorResponse { Message = ex.Message });
        }
    }

    /// <summary>
    /// Revoca un permiso específico de un rol
    /// </summary>
    [HttpDelete("{id}/permissions/{permissionId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RevokePermission(Guid id, Guid permissionId)
    {
        var orgId = GetOrganizationId();
        try
        {
            await _service.RevokePermissionFromRoleAsync(id, permissionId, orgId);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new ErrorResponse { Message = ex.Message });
        }
    }

    // ==================== USER-ROLES ====================

    /// <summary>
    /// Obtiene todas las asignaciones usuario-rol de la organización
    /// </summary>
    [HttpGet("user-assignments")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<UserRoleAssignmentResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserRoleAssignments()
    {
        var orgId = GetOrganizationId();
        var assignments = await _service.GetUserRoleAssignmentsAsync(orgId);
        return Ok(ApiResponse<IEnumerable<UserRoleAssignmentResponse>>.SuccessResponse(
            assignments, "Asignaciones obtenidas exitosamente"));
    }

    /// <summary>
    /// Asigna un rol a un usuario
    /// </summary>
    [HttpPost("assign-to-user")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignRoleToUser([FromBody] AssignRoleToUserRequest request)
    {
        var orgId = GetOrganizationId();
        try
        {
            await _service.AssignRoleToUserAsync(request, orgId);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new ErrorResponse { Message = ex.Message });
        }
    }

    /// <summary>
    /// Revoca un rol de un usuario
    /// </summary>
    [HttpPost("revoke-from-user")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RevokeRoleFromUser([FromBody] RevokeRoleFromUserRequest request)
    {
        var orgId = GetOrganizationId();
        await _service.RevokeRoleFromUserAsync(request, orgId);
        return NoContent();
    }
}