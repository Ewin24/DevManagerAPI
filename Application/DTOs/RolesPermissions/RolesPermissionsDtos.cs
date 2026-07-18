namespace Application.DTOs.RolesPermissions;

// =============================================
// Role DTOs
// =============================================

public class RoleResponse
{
    public Guid Id { get; set; }
    public Guid? OrganizationId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<PermissionResponse> Permissions { get; set; } = new();
}

public class RoleSummaryResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int PermissionCount { get; set; }
    public int UserCount { get; set; }
}

public class CreateRoleRequest
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public List<Guid>? PermissionIds { get; set; }
}

public class UpdateRoleRequest
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}

// =============================================
// Permission DTOs
// =============================================

public class PermissionResponse
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string Module { get; set; } = null!;
}

public class PermissionsByModuleResponse
{
    public string Module { get; set; } = null!;
    public List<PermissionResponse> Permissions { get; set; } = new();
}

public class CreatePermissionRequest
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string Module { get; set; } = null!;
}

public class UpdatePermissionRequest
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string Module { get; set; } = null!;
}

// =============================================
// Assignment DTOs
// =============================================

public class AssignPermissionsToRoleRequest
{
    public List<Guid> PermissionIds { get; set; } = new();
}

public class AssignRoleToUserRequest
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
}

public class RevokeRoleFromUserRequest
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
}

public class AssignPermissionToUserRequest
{
    public Guid UserId { get; set; }
    public Guid PermissionId { get; set; }
    public bool IsGranted { get; set; } = true;
}

// =============================================
// Effective permissions / validation
// =============================================

public class UserEffectivePermissionsResponse
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = null!;
    public List<RoleSummaryResponse> Roles { get; set; } = new();
    public List<PermissionResponse> EffectivePermissions { get; set; } = new();
    public List<UserPermissionOverrideResponse> DirectOverrides { get; set; } = new();
}

public class UserPermissionOverrideResponse
{
    public Guid PermissionId { get; set; }
    public string PermissionCode { get; set; } = null!;
    public string PermissionName { get; set; } = null!;
    public bool IsGranted { get; set; }
}

public class ValidatePermissionRequest
{
    public Guid UserId { get; set; }
    public string PermissionCode { get; set; } = null!;
}

public class ValidatePermissionResponse
{
    public Guid UserId { get; set; }
    public string PermissionCode { get; set; } = null!;
    public bool HasPermission { get; set; }
}

public class UserRoleAssignmentResponse
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public Guid RoleId { get; set; }
    public string RoleName { get; set; } = null!;
    public DateTime AssignedAt { get; set; }
}
