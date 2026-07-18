namespace Domain.Entities.IAM;

using Domain.Common;
using Domain.Enums;

/// <summary>
/// Usuario del sistema con credenciales de autenticación
/// Extiende con campos de persona para el sector solidario
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

    // ===== Persona (sector solidario) =====
    /// <summary>Tipo de persona: Asociado, Empleado o Both</summary>
    public PersonType PersonType { get; set; }

    /// <summary>Tipo de documento de identidad (CC, CE, TI, NIT, etc.)</summary>
    public string? DocumentType { get; set; }

    /// <summary>Número de documento de identidad</summary>
    public string? DocumentNumber { get; set; }

    /// <summary>Fecha de nacimiento</summary>
    public DateTime? BirthDate { get; set; }

    // Navegación
    public Organization? Organization { get; set; }

    /// <summary>Personas asociadas a organizaciones solidarias</summary>
    public ICollection<PersonOrganization> PersonOrganizations { get; set; } = new List<PersonOrganization>();
}
