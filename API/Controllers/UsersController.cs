namespace API.Controllers;

using Application.Common.Models;
using Application.DTOs.Users;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
    /// <remarks>
    /// Retorna el listado completo de usuarios activos (no eliminados) de la organización del usuario autenticado.
    /// 
    /// **Multi-tenancy:** Solo retorna usuarios de la misma organización (filtrado automático por OrganizationId del JWT).
    /// 
    /// **Ejemplo de Response (200 OK):**
    /// 
    ///     {
    ///         "success": true,
    ///         "message": null,
    ///         "data": [
    ///             {
    ///                 "id": "11111111-0000-0000-0000-000000000001",
    ///                 "email": "admin@techcorp.com",
    ///                 "fullName": "Admin TechCorp",
    ///                 "roleName": "Admin",
    ///                 "phoneNumber": "+52 55 1234 5678",
    ///                 "isActive": true,
    ///                 "createdAt": "2024-01-15T10:00:00Z"
    ///             },
    ///             {
    ///                 "id": "11111111-0000-0000-0000-000000000002",
    ///                 "email": "maria.garcia@techcorp.com",
    ///                 "fullName": "María García",
    ///                 "roleName": "Manager",
    ///                 "phoneNumber": null,
    ///                 "isActive": true,
    ///                 "createdAt": "2024-01-20T14:30:00Z"
    ///             }
    ///         ]
    ///     }
    /// 
    /// **Casos de uso:**
    /// - Listado de empleados en dashboard
    /// - Selector de usuarios para asignaciones
    /// - Directorio corporativo
    /// </remarks>
    /// <response code="200">Lista de usuarios obtenida exitosamente</response>
    /// <response code="401">No autenticado - Token inválido o faltante</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<UserResponse>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<UserResponse>>>> GetAll()
    {
        var organizationId = GetOrganizationId();
        var users = await _userService.GetAllAsync(organizationId);
        return Ok(ApiResponse<IEnumerable<UserResponse>>.SuccessResponse(users));
    }

    /// <summary>
    /// Obtiene un usuario específico por ID
    /// </summary>
    /// <remarks>
    /// Retorna la información detallada de un usuario. El usuario debe pertenecer a la misma organización.
    /// 
    /// **Ejemplo de Request:**
    /// 
    ///     GET /api/users/11111111-0000-0000-0000-000000000002
    ///     Authorization: Bearer {token}
    /// 
    /// **Ejemplo de Response (200 OK):**
    /// 
    ///     {
    ///         "success": true,
    ///         "message": null,
    ///         "data": {
    ///             "id": "11111111-0000-0000-0000-000000000002",
    ///             "email": "maria.garcia@techcorp.com",
    ///             "fullName": "María García",
    ///             "roleName": "Manager",
    ///             "phoneNumber": "+52 55 9876 5432",
    ///             "isActive": true,
    ///             "createdAt": "2024-01-20T14:30:00Z"
    ///         }
    ///     }
    /// 
    /// **Casos de uso:**
    /// - Detalle de perfil de usuario
    /// - Verificación de información antes de asignación
    /// - Auditoría de datos
    /// </remarks>
    /// <param name="id">ID del usuario (GUID)</param>
    /// <response code="200">Usuario encontrado exitosamente</response>
    /// <response code="404">Usuario no encontrado o pertenece a otra organización</response>
    /// <response code="401">No autenticado - Token inválido o faltante</response>
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
    /// Crea un nuevo usuario en la organización
    /// </summary>
    /// <remarks>
    /// Registra un nuevo empleado/usuario en la organización. Solo usuarios con rol Admin o Manager pueden crear usuarios.
    /// 
    /// **Ejemplo de Request:**
    /// 
    ///     POST /api/users
    ///     {
    ///         "email": "pedro.lopez@techcorp.com",
    ///         "password": "SecurePass123!",
    ///         "fullName": "Pedro López",
    ///         "roleId": "11111111-0001-0000-0000-000000000003",
    ///         "phoneNumber": "+52 55 1111 2222"
    ///     }
    /// 
    /// **Ejemplo de Response (201 Created):**
    /// 
    ///     {
    ///         "success": true,
    ///         "message": "Usuario creado exitosamente",
    ///         "data": {
    ///             "id": "11111111-0000-0000-0000-000000000005",
    ///             "email": "pedro.lopez@techcorp.com",
    ///             "fullName": "Pedro López",
    ///             "roleName": "Developer",
    ///             "phoneNumber": "+52 55 1111 2222",
    ///             "isActive": true,
    ///             "createdAt": "2026-01-20T16:45:00Z"
    ///         }
    ///     }
    /// 
    /// **Validaciones:**
    /// - Email único dentro de la organización
    /// - Contraseña mínimo 8 caracteres
    /// - RoleId debe existir en la organización
    /// - Campos requeridos: email, password, fullName, roleId
    /// 
    /// **Casos de uso:**
    /// - Onboarding de nuevos empleados
    /// - Gestión de usuarios por administradores
    /// - Creación masiva de usuarios (importación)
    /// </remarks>
    /// <param name="request">Datos del nuevo usuario (email, password, nombre, rol)</param>
    /// <response code="201">Usuario creado exitosamente</response>
    /// <response code="400">Datos inválidos o email duplicado</response>
    /// <response code="401">No autenticado</response>
    /// <response code="403">Sin permisos (requiere rol Admin o Manager)</response>
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
    /// <remarks>
    /// Modifica la información de un usuario. Solo se actualizan los campos enviados (partial update).
    /// 
    /// **Ejemplo de Request:**
    /// 
    ///     PUT /api/users/11111111-0000-0000-0000-000000000002
    ///     {
    ///         "fullName": "María García Sánchez",
    ///         "phoneNumber": "+52 55 9876 5432",
    ///         "roleId": "11111111-0001-0000-0000-000000000002",
    ///         "isActive": true
    ///     }
    /// 
    /// **Ejemplo de Response (200 OK):**
    /// 
    ///     {
    ///         "success": true,
    ///         "message": "Usuario actualizado exitosamente",
    ///         "data": {
    ///             "id": "11111111-0000-0000-0000-000000000002",
    ///             "email": "maria.garcia@techcorp.com",
    ///             "fullName": "María García Sánchez",
    ///             "roleName": "Manager",
    ///             "phoneNumber": "+52 55 9876 5432",
    ///             "isActive": true,
    ///             "createdAt": "2024-01-20T14:30:00Z"
    ///         }
    ///     }
    /// 
    /// **Notas:**
    /// - Solo se actualizan los campos enviados (partial update)
    /// - NO se puede cambiar el email (por seguridad)
    /// - El campo UpdatedAt se actualiza automáticamente
    /// 
    /// **Casos de uso:**
    /// - Actualización de datos de contacto
    /// - Cambio de rol/promoción
    /// - Activación/desactivación de usuarios
    /// </remarks>
    /// <param name="id">ID del usuario a actualizar</param>
    /// <param name="request">Campos a actualizar (fullName, phoneNumber, roleId, isActive)</param>
    /// <response code="200">Usuario actualizado exitosamente</response>
    /// <response code="404">Usuario no encontrado</response>
    /// <response code="400">Datos inválidos</response>
    /// <response code="401">No autenticado</response>
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
    /// Elimina lógicamente un usuario (soft delete)
    /// </summary>
    /// <remarks>
    /// Marca el usuario como eliminado sin borrar físicamente el registro de la base de datos.
    /// 
    /// **Ejemplo de Request:**
    /// 
    ///     DELETE /api/users/11111111-0000-0000-0000-000000000005
    ///     Authorization: Bearer {token}
    /// 
    /// **Response (204 No Content):**
    /// Sin cuerpo de respuesta
    /// 
    /// **Comportamiento:**
    /// - NO elimina físicamente el registro de la base de datos
    /// - Establece IsDeleted = true y DeletedAt = SYSUTCDATETIME()
    /// - Los datos se mantienen para auditoría e integridad referencial
    /// - El usuario NO podrá hacer login después de eliminado
    /// - Los proyectos/asignaciones previas se mantienen intactas
    /// 
    /// **Casos de uso:**
    /// - Baja de empleados
    /// - Limpieza de usuarios inactivos
    /// - Revocación de acceso
    /// </remarks>
    /// <param name="id">ID del usuario a eliminar</param>
    /// <response code="204">Usuario eliminado exitosamente (sin contenido)</response>
    /// <response code="404">Usuario no encontrado</response>
    /// <response code="401">No autenticado</response>
    /// <response code="403">Sin permisos (requiere rol Admin o Manager)</response>
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
