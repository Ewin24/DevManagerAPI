namespace Domain.Entities.IAM;

/// <summary>
/// Tabla pivote para relación muchos-a-muchos User-Role
/// </summary>
public class UserRole
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public Guid OrganizationId { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navegación
    public User? User { get; set; }
    public Role? Role { get; set; }
    public Organization? Organization { get; set; }
}
