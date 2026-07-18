namespace Domain.Interfaces.Repositories;

using Domain.Entities.IAM;

/// <summary>
/// Repositorio para gestión de roles, permisos y asignaciones RBAC
/// </summary>
public interface IRolePermissionRepository
{
    // === Roles ===
    Task<IEnumerable<Role>> GetAllRolesAsync(Guid organizationId);
    Task<Role?> GetRoleByIdAsync(Guid roleId, Guid organizationId);
    Task<Guid> CreateRoleAsync(Role role);
    Task<bool> UpdateRoleAsync(Role role);
    Task<bool> SoftDeleteRoleAsync(Guid roleId, Guid organizationId);
    Task<bool> RoleNameExistsAsync(string name, Guid organizationId, Guid? excludeRoleId = null);

    // === Permissions ===
    Task<IEnumerable<Permission>> GetAllPermissionsAsync();
    Task<IEnumerable<Permission>> GetPermissionsByModuleAsync(string module);
    Task<Permission?> GetPermissionByIdAsync(Guid permissionId);
    Task<Guid> CreatePermissionAsync(Permission permission);
    Task<bool> UpdatePermissionAsync(Permission permission);
    Task<bool> SoftDeletePermissionAsync(Guid permissionId);
    Task<bool> PermissionCodeExistsAsync(string code, Guid? excludePermissionId = null);

    // === RolePermissions ===
    Task<IEnumerable<Permission>> GetPermissionsByRoleAsync(Guid roleId);
    Task<bool> AssignPermissionToRoleAsync(Guid roleId, Guid permissionId);
    Task<bool> RevokePermissionFromRoleAsync(Guid roleId, Guid permissionId);
    Task<bool> SetRolePermissionsAsync(Guid roleId, IEnumerable<Guid> permissionIds);

    // === UserRoles ===
    Task<IEnumerable<Role>> GetRolesByUserAsync(Guid userId, Guid organizationId);
    Task<bool> AssignRoleToUserAsync(Guid userId, Guid roleId, Guid organizationId);
    Task<bool> RevokeRoleFromUserAsync(Guid userId, Guid roleId);
    Task<IEnumerable<UserRole>> GetUserRoleAssignmentsAsync(Guid organizationId);
    Task<int> GetUserCountByRoleAsync(Guid roleId, Guid organizationId);

    // === UserPermissions (overrides directos) ===
    Task<IEnumerable<UserPermission>> GetDirectPermissionsByUserAsync(Guid userId, Guid organizationId);
    Task<bool> AssignPermissionToUserAsync(UserPermission userPermission);
    Task<bool> RevokePermissionFromUserAsync(Guid userId, Guid permissionId);

    // === Validación de permisos efectivos ===
    Task<IEnumerable<Permission>> GetEffectivePermissionsAsync(Guid userId, Guid organizationId);
    Task<bool> UserHasPermissionAsync(Guid userId, Guid organizationId, string permissionCode);
}
