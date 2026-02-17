namespace Domain.Entities.IAM;

/// <summary>
/// Permiso granular del sistema para control de acceso (RBAC)
/// </summary>
public class Permission
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;       // e.g. "users.create", "projects.delete"
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string Module { get; set; } = null!;      // Agrupación: "users", "projects", "talent", etc.
    public DateTime CreatedAt { get; set; }
    public bool IsDeleted { get; set; }
}
