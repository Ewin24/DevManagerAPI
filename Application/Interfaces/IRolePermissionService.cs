namespace Application.Interfaces;

using Application.DTOs.RolesPermissions;

/// <summary>
/// Servicio de gestión de roles, permisos y asignaciones RBAC
/// </summary>
public interface IRolePermissionService
{
    // === Roles ===
    Task<IEnumerable<RoleSummaryResponse>> GetAllRolesAsync(Guid organizationId);
    Task<RoleResponse> GetRoleByIdAsync(Guid roleId, Guid organizationId);
    Task<RoleResponse> CreateRoleAsync(CreateRoleRequest request, Guid organizationId);
    Task<RoleResponse> UpdateRoleAsync(Guid roleId, UpdateRoleRequest request, Guid organizationId);
    Task DeleteRoleAsync(Guid roleId, Guid organizationId);

    // === Permissions ===
    Task<IEnumerable<PermissionsByModuleResponse>> GetAllPermissionsGroupedAsync();
    Task<IEnumerable<PermissionResponse>> GetAllPermissionsAsync();
    Task<PermissionResponse> GetPermissionByIdAsync(Guid permissionId);
    Task<PermissionResponse> CreatePermissionAsync(CreatePermissionRequest request);
    Task<PermissionResponse> UpdatePermissionAsync(Guid permissionId, UpdatePermissionRequest request);
    Task DeletePermissionAsync(Guid permissionId);

    // === Role-Permission assignments ===
    Task<RoleResponse> GetRoleWithPermissionsAsync(Guid roleId, Guid organizationId);
    Task AssignPermissionsToRoleAsync(Guid roleId, AssignPermissionsToRoleRequest request, Guid organizationId);
    Task RevokePermissionFromRoleAsync(Guid roleId, Guid permissionId, Guid organizationId);

    // === User-Role assignments ===
    Task<IEnumerable<UserRoleAssignmentResponse>> GetUserRoleAssignmentsAsync(Guid organizationId);
    Task AssignRoleToUserAsync(AssignRoleToUserRequest request, Guid organizationId);
    Task RevokeRoleFromUserAsync(RevokeRoleFromUserRequest request, Guid organizationId);

    // === User-Permission direct overrides ===
    Task AssignPermissionToUserAsync(AssignPermissionToUserRequest request, Guid organizationId);
    Task RevokePermissionFromUserAsync(Guid userId, Guid permissionId, Guid organizationId);

    // === Permission validation ===
    Task<UserEffectivePermissionsResponse> GetUserEffectivePermissionsAsync(Guid userId, Guid organizationId);
    Task<ValidatePermissionResponse> ValidatePermissionAsync(ValidatePermissionRequest request, Guid organizationId);
}
