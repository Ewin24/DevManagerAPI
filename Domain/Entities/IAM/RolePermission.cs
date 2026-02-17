namespace Domain.Entities.IAM;

/// <summary>
/// Tabla pivote para relación muchos-a-muchos Role-Permission
/// </summary>
public class RolePermission
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navegación
    public Role? Role { get; set; }
    public Permission? Permission { get; set; }
}
