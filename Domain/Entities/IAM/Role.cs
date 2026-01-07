namespace Domain.Entities.IAM;

/// <summary>
/// Rol para control de acceso basado en roles (RBAC)
/// </summary>
public class Role
{
    public Guid Id { get; set; }
    public Guid? OrganizationId { get; set; } // NULL = rol global del sistema
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsDeleted { get; set; }

    // Navegación
    public Organization? Organization { get; set; }
}
