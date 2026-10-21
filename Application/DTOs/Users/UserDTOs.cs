namespace Application.DTOs.Users;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// DTO para crear un nuevo usuario
/// </summary>
public class CreateUserRequest
{
    [Required]
    [MaxLength(80)]
    public string FirstName { get; set; } = null!;

    [Required]
    [MaxLength(80)]
    public string LastName { get; set; } = null!;

    [Required]
    [EmailAddress]
    [MaxLength(254)]
    public string Email { get; set; } = null!;

    [MaxLength(30)]
    public string? Phone { get; set; }

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = null!;

    /// <summary>Bandera explícita de empleado (por defecto true)</summary>
    public bool IsEmployee { get; set; } = true;
}

/// <summary>
/// DTO para actualizar usuario
/// </summary>
public class UpdateUserRequest
{
    [Required]
    [MaxLength(80)]
    public string FirstName { get; set; } = null!;

    [Required]
    [MaxLength(80)]
    public string LastName { get; set; } = null!;

    [MaxLength(30)]
    public string? Phone { get; set; }

    public bool? IsActive { get; set; }

    /// <summary>Bandera explícita de empleado; si se omite no cambia</summary>
    public bool? IsEmployee { get; set; }
}

/// <summary>
/// DTO de respuesta con datos del usuario
/// </summary>
public class UserResponse
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public bool IsActive { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public Domain.Enums.DocumentType? DocumentType { get; set; }
    public string? DocumentNumber { get; set; }
    public bool IsEmployee { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public Guid? RoleId { get; set; }
    /// <summary>
    /// Nombre del rol asignado. Nunca será null; si no tiene rol asignado, será "Sin asignar"
    /// </summary>
    public string RoleName { get; set; } = "Sin asignar";
}

/// <summary>
/// DTO para fijar el documento de identidad (requiere consentimiento de datos activo)
/// </summary>
public class SetDocumentRequest
{
    public Domain.Enums.DocumentType DocumentType { get; set; }

    [Required]
    [MaxLength(20)]
    public string DocumentNumber { get; set; } = null!;
}
