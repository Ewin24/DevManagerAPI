namespace Domain.Entities.IAM;

using Domain.Common;

/// <summary>
/// Usuario del sistema con credenciales de autenticación
/// </summary>
public class User : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public byte[] PasswordHash { get; set; } = null!;
    public byte[] PasswordSalt { get; set; } = null!;
    public bool IsActive { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public Domain.Enums.DocumentType? DocumentType { get; set; }
    public string? DocumentNumber { get; set; }
    /// <summary>Bandera explícita de empleado (los usuarios existentes son empleados)</summary>
    public bool IsEmployee { get; set; } = true;

    // Navegación
    public Organization? Organization { get; set; }
}
