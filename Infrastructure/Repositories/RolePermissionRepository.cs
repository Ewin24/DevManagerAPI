namespace Infrastructure.Repositories;

using Domain.Entities.IAM;
using Domain.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

/// <summary>
/// Implementación del repositorio RBAC usando Entity Framework Core
/// </summary>
public class RolePermissionRepository : IRolePermissionRepository
{
    private readonly DevManagerDbContext _context;
    private readonly ILogger<RolePermissionRepository> _logger;

    public RolePermissionRepository(DevManagerDbContext context, ILogger<RolePermissionRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    // ==================== ROLES ====================

    public async Task<IEnumerable<Role>> GetAllRolesAsync(Guid organizationId)
    {
        var efRoles = await _context.Roles
            .AsNoTracking()
            .Where(r => (r.OrganizationId == organizationId || r.OrganizationId == null) && !r.IsDeleted)
            .OrderBy(r => r.Name)
            .ToListAsync();

        return efRoles.Select(MapRoleToDomain);
    }

    public async Task<Role?> GetRoleByIdAsync(Guid roleId, Guid organizationId)
    {
        var efRole = await _context.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(r =>
                r.Id == roleId &&
                (r.OrganizationId == organizationId || r.OrganizationId == null) &&
                !r.IsDeleted);

        return efRole == null ? null : MapRoleToDomain(efRole);
    }

    public async Task<Guid> CreateRoleAsync(Role role)
    {
        var efRole = new Data.Entities.Role
        {
            Id = role.Id,
            OrganizationId = role.OrganizationId,
            Name = role.Name,
            Description = role.Description,
            CreatedAt = role.CreatedAt,
            IsDeleted = false
        };

        _context.Roles.Add(efRole);
        await _context.SaveChangesAsync();
        return efRole.Id;
    }

    public async Task<bool> UpdateRoleAsync(Role role)
    {
        var efRole = await _context.Roles
            .FirstOrDefaultAsync(r => r.Id == role.Id && !r.IsDeleted);

        if (efRole == null) return false;

        efRole.Name = role.Name;
        efRole.Description = role.Description;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SoftDeleteRoleAsync(Guid roleId, Guid organizationId)
    {
        var efRole = await _context.Roles
            .FirstOrDefaultAsync(r =>
                r.Id == roleId &&
                r.OrganizationId == organizationId &&
                !r.IsDeleted);

        if (efRole == null) return false;

        efRole.IsDeleted = true;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RoleNameExistsAsync(string name, Guid organizationId, Guid? excludeRoleId = null)
    {
        return await _context.Roles.AnyAsync(r =>
            r.Name == name &&
            (r.OrganizationId == organizationId || r.OrganizationId == null) &&
            !r.IsDeleted &&
            (excludeRoleId == null || r.Id != excludeRoleId));
    }

    // ==================== PERMISSIONS ====================

    public async Task<IEnumerable<Permission>> GetAllPermissionsAsync()
    {
        var efPerms = await _context.Permissions
            .AsNoTracking()
            .Where(p => !p.IsDeleted)
            .OrderBy(p => p.Module)
            .ThenBy(p => p.Code)
            .ToListAsync();

        return efPerms.Select(MapPermissionToDomain);
    }

    public async Task<IEnumerable<Permission>> GetPermissionsByModuleAsync(string module)
    {
        var efPerms = await _context.Permissions
            .AsNoTracking()
            .Where(p => p.Module == module && !p.IsDeleted)
            .OrderBy(p => p.Code)
            .ToListAsync();

        return efPerms.Select(MapPermissionToDomain);
    }

    public async Task<Permission?> GetPermissionByIdAsync(Guid permissionId)
    {
        var efPerm = await _context.Permissions
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == permissionId && !p.IsDeleted);

        return efPerm == null ? null : MapPermissionToDomain(efPerm);
    }

    public async Task<Guid> CreatePermissionAsync(Permission permission)
    {
        var efPerm = new Data.Entities.Permission
        {
            Id = permission.Id,
            Code = permission.Code,
            Name = permission.Name,
            Description = permission.Description,
            Module = permission.Module,
            CreatedAt = permission.CreatedAt,
            IsDeleted = false
        };

        _context.Permissions.Add(efPerm);
        await _context.SaveChangesAsync();
        return efPerm.Id;
    }

    public async Task<bool> UpdatePermissionAsync(Permission permission)
    {
        var efPerm = await _context.Permissions
            .FirstOrDefaultAsync(p => p.Id == permission.Id && !p.IsDeleted);

        if (efPerm == null) return false;

        efPerm.Name = permission.Name;
        efPerm.Description = permission.Description;
        efPerm.Module = permission.Module;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SoftDeletePermissionAsync(Guid permissionId)
    {
        var efPerm = await _context.Permissions
            .FirstOrDefaultAsync(p => p.Id == permissionId && !p.IsDeleted);

        if (efPerm == null) return false;

        efPerm.IsDeleted = true;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> PermissionCodeExistsAsync(string code, Guid? excludePermissionId = null)
    {
        return await _context.Permissions.AnyAsync(p =>
            p.Code == code &&
            !p.IsDeleted &&
            (excludePermissionId == null || p.Id != excludePermissionId));
    }

    // ==================== ROLE-PERMISSIONS ====================

    public async Task<IEnumerable<Permission>> GetPermissionsByRoleAsync(Guid roleId)
    {
        var efPerms = await _context.RolePermissions
            .AsNoTracking()
            .Where(rp => rp.RoleId == roleId)
            .Include(rp => rp.Permission)
            .Where(rp => !rp.Permission.IsDeleted)
            .Select(rp => rp.Permission)
            .OrderBy(p => p.Module)
            .ThenBy(p => p.Code)
            .ToListAsync();

        return efPerms.Select(MapPermissionToDomain);
    }

    public async Task<bool> AssignPermissionToRoleAsync(Guid roleId, Guid permissionId)
    {
        var exists = await _context.RolePermissions
            .AnyAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);

        if (exists) return true; // ya asignado

        _context.RolePermissions.Add(new Data.Entities.RolePermission
        {
            RoleId = roleId,
            PermissionId = permissionId,
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RevokePermissionFromRoleAsync(Guid roleId, Guid permissionId)
    {
        var efRp = await _context.RolePermissions
            .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);

        if (efRp == null) return false;

        _context.RolePermissions.Remove(efRp);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SetRolePermissionsAsync(Guid roleId, IEnumerable<Guid> permissionIds)
    {
        // Eliminar permisos actuales
        var currentPerms = await _context.RolePermissions
            .Where(rp => rp.RoleId == roleId)
            .ToListAsync();

        _context.RolePermissions.RemoveRange(currentPerms);

        // Agregar nuevos
        var newPerms = permissionIds.Select(pid => new Data.Entities.RolePermission
        {
            RoleId = roleId,
            PermissionId = pid,
            CreatedAt = DateTime.UtcNow
        });

        _context.RolePermissions.AddRange(newPerms);
        await _context.SaveChangesAsync();
        return true;
    }

    // ==================== USER-ROLES ====================

    public async Task<IEnumerable<Role>> GetRolesByUserAsync(Guid userId, Guid organizationId)
    {
        var efRoles = await _context.UserRoles
            .AsNoTracking()
            .Where(ur => ur.UserId == userId && ur.OrganizationId == organizationId)
            .Include(ur => ur.Role)
            .Where(ur => !ur.Role.IsDeleted)
            .Select(ur => ur.Role)
            .ToListAsync();

        return efRoles.Select(MapRoleToDomain);
    }

    public async Task<bool> AssignRoleToUserAsync(Guid userId, Guid roleId, Guid organizationId)
    {
        var exists = await _context.UserRoles
            .AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

        if (exists) return true;

        _context.UserRoles.Add(new Data.Entities.UserRole
        {
            UserId = userId,
            RoleId = roleId,
            OrganizationId = organizationId,
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RevokeRoleFromUserAsync(Guid userId, Guid roleId)
    {
        var efUr = await _context.UserRoles
            .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

        if (efUr == null) return false;

        _context.UserRoles.Remove(efUr);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<UserRole>> GetUserRoleAssignmentsAsync(Guid organizationId)
    {
        var efAssignments = await _context.UserRoles
            .AsNoTracking()
            .Where(ur => ur.OrganizationId == organizationId)
            .Include(ur => ur.User)
            .Include(ur => ur.Role)
            .Where(ur => !ur.User.IsDeleted && !ur.Role.IsDeleted)
            .ToListAsync();

        return efAssignments.Select(ur => new UserRole
        {
            UserId = ur.UserId,
            RoleId = ur.RoleId,
            OrganizationId = ur.OrganizationId,
            CreatedAt = ur.CreatedAt,
            User = new User
            {
                Id = ur.User.Id,
                FirstName = ur.User.FirstName,
                LastName = ur.User.LastName,
                Email = ur.User.Email,
                OrganizationId = ur.User.OrganizationId
            },
            Role = new Role
            {
                Id = ur.Role.Id,
                Name = ur.Role.Name,
                Description = ur.Role.Description
            }
        });
    }

    public async Task<int> GetUserCountByRoleAsync(Guid roleId, Guid organizationId)
    {
        return await _context.UserRoles
            .AsNoTracking()
            .Where(ur => ur.RoleId == roleId && ur.OrganizationId == organizationId)
            .Where(ur => !ur.User.IsDeleted)
            .CountAsync();
    }

    // ==================== USER-PERMISSIONS (overrides directos) ====================

    public async Task<IEnumerable<UserPermission>> GetDirectPermissionsByUserAsync(Guid userId, Guid organizationId)
    {
        var efUps = await _context.UserPermissions
            .AsNoTracking()
            .Where(up => up.UserId == userId && up.OrganizationId == organizationId)
            .Include(up => up.Permission)
            .Where(up => !up.Permission.IsDeleted)
            .ToListAsync();

        return efUps.Select(up => new UserPermission
        {
            UserId = up.UserId,
            PermissionId = up.PermissionId,
            OrganizationId = up.OrganizationId,
            IsGranted = up.IsGranted,
            CreatedAt = up.CreatedAt,
            Permission = MapPermissionToDomain(up.Permission)
        });
    }

    public async Task<bool> AssignPermissionToUserAsync(UserPermission userPermission)
    {
        var existing = await _context.UserPermissions
            .FirstOrDefaultAsync(up =>
                up.UserId == userPermission.UserId &&
                up.PermissionId == userPermission.PermissionId);

        if (existing != null)
        {
            existing.IsGranted = userPermission.IsGranted;
            existing.OrganizationId = userPermission.OrganizationId;
        }
        else
        {
            _context.UserPermissions.Add(new Data.Entities.UserPermission
            {
                UserId = userPermission.UserId,
                PermissionId = userPermission.PermissionId,
                OrganizationId = userPermission.OrganizationId,
                IsGranted = userPermission.IsGranted,
                CreatedAt = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RevokePermissionFromUserAsync(Guid userId, Guid permissionId)
    {
        var efUp = await _context.UserPermissions
            .FirstOrDefaultAsync(up => up.UserId == userId && up.PermissionId == permissionId);

        if (efUp == null) return false;

        _context.UserPermissions.Remove(efUp);
        await _context.SaveChangesAsync();
        return true;
    }

    // ==================== EFFECTIVE PERMISSIONS ====================

    public async Task<IEnumerable<Permission>> GetEffectivePermissionsAsync(Guid userId, Guid organizationId)
    {
        // 1. Obtener permisos de todos los roles del usuario
        var rolePermissions = await _context.UserRoles
            .AsNoTracking()
            .Where(ur => ur.UserId == userId && ur.OrganizationId == organizationId)
            .SelectMany(ur => ur.Role.RolePermissions)
            .Where(rp => !rp.Permission.IsDeleted)
            .Select(rp => rp.Permission)
            .Distinct()
            .ToListAsync();

        // 2. Obtener permisos directos del usuario
        var directPermissions = await _context.UserPermissions
            .AsNoTracking()
            .Where(up => up.UserId == userId && up.OrganizationId == organizationId)
            .Include(up => up.Permission)
            .Where(up => !up.Permission.IsDeleted)
            .ToListAsync();

        // 3. Construir mapa de permisos efectivos
        var effectiveMap = new Dictionary<Guid, Data.Entities.Permission>();

        // Agregar permisos de roles
        foreach (var perm in rolePermissions)
        {
            effectiveMap[perm.Id] = perm;
        }

        // Aplicar overrides directos
        foreach (var dp in directPermissions)
        {
            if (dp.IsGranted)
            {
                effectiveMap[dp.PermissionId] = dp.Permission;
            }
            else
            {
                effectiveMap.Remove(dp.PermissionId);
            }
        }

        return effectiveMap.Values
            .OrderBy(p => p.Module)
            .ThenBy(p => p.Code)
            .Select(MapPermissionToDomain);
    }

    public async Task<bool> UserHasPermissionAsync(Guid userId, Guid organizationId, string permissionCode)
    {
        // Verificar denegación explícita primero
        var explicitDeny = await _context.UserPermissions
            .AsNoTracking()
            .AnyAsync(up =>
                up.UserId == userId &&
                up.OrganizationId == organizationId &&
                up.Permission.Code == permissionCode &&
                !up.Permission.IsDeleted &&
                !up.IsGranted);

        if (explicitDeny) return false;

        // Verificar permiso directo concedido
        var directGrant = await _context.UserPermissions
            .AsNoTracking()
            .AnyAsync(up =>
                up.UserId == userId &&
                up.OrganizationId == organizationId &&
                up.Permission.Code == permissionCode &&
                !up.Permission.IsDeleted &&
                up.IsGranted);

        if (directGrant) return true;

        // Verificar a través de roles
        return await _context.UserRoles
            .AsNoTracking()
            .Where(ur => ur.UserId == userId && ur.OrganizationId == organizationId)
            .SelectMany(ur => ur.Role.RolePermissions)
            .AnyAsync(rp =>
                rp.Permission.Code == permissionCode &&
                !rp.Permission.IsDeleted);
    }

    // ==================== MAPPING HELPERS ====================

    private static Role MapRoleToDomain(Data.Entities.Role efRole)
    {
        return new Role
        {
            Id = efRole.Id,
            OrganizationId = efRole.OrganizationId,
            Name = efRole.Name,
            Description = efRole.Description,
            CreatedAt = efRole.CreatedAt,
            IsDeleted = efRole.IsDeleted
        };
    }

    private static Permission MapPermissionToDomain(Data.Entities.Permission efPerm)
    {
        return new Permission
        {
            Id = efPerm.Id,
            Code = efPerm.Code,
            Name = efPerm.Name,
            Description = efPerm.Description,
            Module = efPerm.Module,
            CreatedAt = efPerm.CreatedAt,
            IsDeleted = efPerm.IsDeleted
        };
    }
}