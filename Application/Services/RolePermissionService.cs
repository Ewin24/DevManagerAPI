namespace Application.Services;

using Application.Common.Exceptions;
using Application.DTOs.RolesPermissions;
using Application.Interfaces;
using Domain.Entities.IAM;
using Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

/// <summary>
/// Implementación del servicio de gestión de roles y permisos RBAC
/// </summary>
public class RolePermissionService : IRolePermissionService
{
    private readonly IRolePermissionRepository _repository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<RolePermissionService> _logger;

    public RolePermissionService(
        IRolePermissionRepository repository,
        IUserRepository userRepository,
        ILogger<RolePermissionService> logger)
    {
        _repository = repository;
        _userRepository = userRepository;
        _logger = logger;
    }

    // ==================== ROLES ====================

    public async Task<IEnumerable<RoleSummaryResponse>> GetAllRolesAsync(Guid organizationId)
    {
        var roles = await _repository.GetAllRolesAsync(organizationId);
        var result = new List<RoleSummaryResponse>();

        foreach (var role in roles)
        {
            var permissions = await _repository.GetPermissionsByRoleAsync(role.Id);
            var userCount = await _repository.GetUserCountByRoleAsync(role.Id, organizationId);
            result.Add(new RoleSummaryResponse
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                PermissionCount = permissions.Count(),
                UserCount = userCount
            });
        }

        return result;
    }

    public async Task<RoleResponse> GetRoleByIdAsync(Guid roleId, Guid organizationId)
    {
        var role = await _repository.GetRoleByIdAsync(roleId, organizationId)
            ?? throw new NotFoundException("Rol", roleId);

        var permissions = await _repository.GetPermissionsByRoleAsync(roleId);

        return new RoleResponse
        {
            Id = role.Id,
            OrganizationId = role.OrganizationId,
            Name = role.Name,
            Description = role.Description,
            CreatedAt = role.CreatedAt,
            Permissions = permissions.Select(MapPermissionToResponse).ToList()
        };
    }

    public async Task<RoleResponse> CreateRoleAsync(CreateRoleRequest request, Guid organizationId)
    {
        if (await _repository.RoleNameExistsAsync(request.Name, organizationId))
            throw new ConflictException($"El rol '{request.Name}' ya existe");

        var role = new Role
        {
            Id = Guid.NewGuid(),
            OrganizationId = organizationId,
            Name = request.Name,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.CreateRoleAsync(role);

        // Asignar permisos iniciales si se proporcionan
        if (request.PermissionIds?.Any() == true)
        {
            await _repository.SetRolePermissionsAsync(role.Id, request.PermissionIds);
        }

        _logger.LogInformation("Rol creado: {RoleId} - {Name}", role.Id, role.Name);

        return await GetRoleByIdAsync(role.Id, organizationId);
    }

    public async Task<RoleResponse> UpdateRoleAsync(Guid roleId, UpdateRoleRequest request, Guid organizationId)
    {
        var role = await _repository.GetRoleByIdAsync(roleId, organizationId)
            ?? throw new NotFoundException("Rol", roleId);

        if (await _repository.RoleNameExistsAsync(request.Name, organizationId, roleId))
            throw new ConflictException($"El rol '{request.Name}' ya existe");

        role.Name = request.Name;
        role.Description = request.Description;

        await _repository.UpdateRoleAsync(role);

        _logger.LogInformation("Rol actualizado: {RoleId}", roleId);

        return await GetRoleByIdAsync(roleId, organizationId);
    }

    public async Task DeleteRoleAsync(Guid roleId, Guid organizationId)
    {
        var role = await _repository.GetRoleByIdAsync(roleId, organizationId)
            ?? throw new NotFoundException("Rol", roleId);

        if (role.OrganizationId == null)
            throw new BusinessValidationException("No se pueden eliminar roles globales del sistema");

        await _repository.SoftDeleteRoleAsync(roleId, organizationId);

        _logger.LogInformation("Rol eliminado: {RoleId}", roleId);
    }

    // ==================== PERMISSIONS ====================

    public async Task<IEnumerable<PermissionsByModuleResponse>> GetAllPermissionsGroupedAsync()
    {
        var permissions = await _repository.GetAllPermissionsAsync();

        return permissions
            .GroupBy(p => p.Module)
            .OrderBy(g => g.Key)
            .Select(g => new PermissionsByModuleResponse
            {
                Module = g.Key,
                Permissions = g.Select(MapPermissionToResponse).ToList()
            });
    }

    public async Task<IEnumerable<PermissionResponse>> GetAllPermissionsAsync()
    {
        var permissions = await _repository.GetAllPermissionsAsync();
        return permissions.Select(MapPermissionToResponse);
    }

    public async Task<PermissionResponse> GetPermissionByIdAsync(Guid permissionId)
    {
        var permission = await _repository.GetPermissionByIdAsync(permissionId)
            ?? throw new NotFoundException("Permiso", permissionId);

        return MapPermissionToResponse(permission);
    }

    public async Task<PermissionResponse> CreatePermissionAsync(CreatePermissionRequest request)
    {
        if (await _repository.PermissionCodeExistsAsync(request.Code))
            throw new ConflictException($"El código de permiso '{request.Code}' ya existe");

        var permission = new Permission
        {
            Id = Guid.NewGuid(),
            Code = request.Code,
            Name = request.Name,
            Description = request.Description,
            Module = request.Module,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.CreatePermissionAsync(permission);

        _logger.LogInformation("Permiso creado: {PermissionId} - {Code}", permission.Id, permission.Code);

        return MapPermissionToResponse(permission);
    }

    public async Task<PermissionResponse> UpdatePermissionAsync(Guid permissionId, UpdatePermissionRequest request)
    {
        var permission = await _repository.GetPermissionByIdAsync(permissionId)
            ?? throw new NotFoundException("Permiso", permissionId);

        permission.Name = request.Name;
        permission.Description = request.Description;
        permission.Module = request.Module;

        await _repository.UpdatePermissionAsync(permission);

        _logger.LogInformation("Permiso actualizado: {PermissionId}", permissionId);

        return MapPermissionToResponse(permission);
    }

    public async Task DeletePermissionAsync(Guid permissionId)
    {
        var permission = await _repository.GetPermissionByIdAsync(permissionId)
            ?? throw new NotFoundException("Permiso", permissionId);

        await _repository.SoftDeletePermissionAsync(permissionId);

        _logger.LogInformation("Permiso eliminado: {PermissionId}", permissionId);
    }

    // ==================== ROLE-PERMISSION ASSIGNMENTS ====================

    public async Task<RoleResponse> GetRoleWithPermissionsAsync(Guid roleId, Guid organizationId)
    {
        return await GetRoleByIdAsync(roleId, organizationId);
    }

    public async Task AssignPermissionsToRoleAsync(Guid roleId, AssignPermissionsToRoleRequest request, Guid organizationId)
    {
        var role = await _repository.GetRoleByIdAsync(roleId, organizationId)
            ?? throw new NotFoundException("Rol", roleId);

        await _repository.SetRolePermissionsAsync(roleId, request.PermissionIds);

        _logger.LogInformation("Permisos asignados al rol {RoleId}: {Count} permisos", roleId, request.PermissionIds.Count);
    }

    public async Task RevokePermissionFromRoleAsync(Guid roleId, Guid permissionId, Guid organizationId)
    {
        var role = await _repository.GetRoleByIdAsync(roleId, organizationId)
            ?? throw new NotFoundException("Rol", roleId);

        await _repository.RevokePermissionFromRoleAsync(roleId, permissionId);

        _logger.LogInformation("Permiso {PermissionId} revocado del rol {RoleId}", permissionId, roleId);
    }

    // ==================== USER-ROLE ASSIGNMENTS ====================

    public async Task<IEnumerable<UserRoleAssignmentResponse>> GetUserRoleAssignmentsAsync(Guid organizationId)
    {
        var assignments = await _repository.GetUserRoleAssignmentsAsync(organizationId);

        return assignments.Select(a => new UserRoleAssignmentResponse
        {
            UserId = a.UserId,
            UserName = $"{a.User?.FirstName} {a.User?.LastName}",
            Email = a.User?.Email ?? "",
            RoleId = a.RoleId,
            RoleName = a.Role?.Name ?? "",
            AssignedAt = a.CreatedAt
        });
    }

    public async Task AssignRoleToUserAsync(AssignRoleToUserRequest request, Guid organizationId)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, organizationId)
            ?? throw new NotFoundException("Usuario", request.UserId);

        var role = await _repository.GetRoleByIdAsync(request.RoleId, organizationId)
            ?? throw new NotFoundException("Rol", request.RoleId);

        await _repository.AssignRoleToUserAsync(request.UserId, request.RoleId, organizationId);

        _logger.LogInformation("Rol {RoleId} asignado al usuario {UserId}", request.RoleId, request.UserId);
    }

    public async Task RevokeRoleFromUserAsync(RevokeRoleFromUserRequest request, Guid organizationId)
    {
        await _repository.RevokeRoleFromUserAsync(request.UserId, request.RoleId);

        _logger.LogInformation("Rol {RoleId} revocado del usuario {UserId}", request.RoleId, request.UserId);
    }

    // ==================== USER-PERMISSION DIRECT OVERRIDES ====================

    public async Task AssignPermissionToUserAsync(AssignPermissionToUserRequest request, Guid organizationId)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, organizationId)
            ?? throw new NotFoundException("Usuario", request.UserId);

        var permission = await _repository.GetPermissionByIdAsync(request.PermissionId)
            ?? throw new NotFoundException("Permiso", request.PermissionId);

        var userPermission = new UserPermission
        {
            UserId = request.UserId,
            PermissionId = request.PermissionId,
            OrganizationId = organizationId,
            IsGranted = request.IsGranted,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AssignPermissionToUserAsync(userPermission);

        _logger.LogInformation(
            "Permiso {PermissionId} ({Action}) asignado al usuario {UserId}",
            request.PermissionId, request.IsGranted ? "GRANT" : "DENY", request.UserId);
    }

    public async Task RevokePermissionFromUserAsync(Guid userId, Guid permissionId, Guid organizationId)
    {
        await _repository.RevokePermissionFromUserAsync(userId, permissionId);

        _logger.LogInformation("Override de permiso {PermissionId} removido del usuario {UserId}", permissionId, userId);
    }

    // ==================== VALIDATION ====================

    public async Task<UserEffectivePermissionsResponse> GetUserEffectivePermissionsAsync(Guid userId, Guid organizationId)
    {
        var user = await _userRepository.GetByIdAsync(userId, organizationId)
            ?? throw new NotFoundException("Usuario", userId);

        var roles = await _repository.GetRolesByUserAsync(userId, organizationId);
        var effectivePerms = await _repository.GetEffectivePermissionsAsync(userId, organizationId);
        var directOverrides = await _repository.GetDirectPermissionsByUserAsync(userId, organizationId);

        return new UserEffectivePermissionsResponse
        {
            UserId = userId,
            UserName = $"{user.FirstName} {user.LastName}",
            Roles = roles.Select(r => new RoleSummaryResponse
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description
            }).ToList(),
            EffectivePermissions = effectivePerms.Select(MapPermissionToResponse).ToList(),
            DirectOverrides = directOverrides.Select(up => new UserPermissionOverrideResponse
            {
                PermissionId = up.PermissionId,
                PermissionCode = up.Permission?.Code ?? "",
                PermissionName = up.Permission?.Name ?? "",
                IsGranted = up.IsGranted
            }).ToList()
        };
    }

    public async Task<ValidatePermissionResponse> ValidatePermissionAsync(ValidatePermissionRequest request, Guid organizationId)
    {
        var hasPermission = await _repository.UserHasPermissionAsync(
            request.UserId, organizationId, request.PermissionCode);

        return new ValidatePermissionResponse
        {
            UserId = request.UserId,
            PermissionCode = request.PermissionCode,
            HasPermission = hasPermission
        };
    }

    // ==================== HELPERS ====================

    private static PermissionResponse MapPermissionToResponse(Permission permission)
    {
        return new PermissionResponse
        {
            Id = permission.Id,
            Code = permission.Code,
            Name = permission.Name,
            Description = permission.Description,
            Module = permission.Module
        };
    }
}
