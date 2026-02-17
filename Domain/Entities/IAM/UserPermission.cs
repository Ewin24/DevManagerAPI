namespace Domain.Entities.IAM;

/// <summary>
/// Permisos directos asignados a un usuario (override sobre permisos del rol)
/// </summary>
public class UserPermission
{
    public Guid UserId { get; set; }
    public Guid PermissionId { get; set; }
    public Guid OrganizationId { get; set; }
    public bool IsGranted { get; set; } = true; // true = permitir, false = denegar explícitamente
    public DateTime CreatedAt { get; set; }

    // Navegación
    public User? User { get; set; }
    public Permission? Permission { get; set; }
    public Organization? Organization { get; set; }
}
