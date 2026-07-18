namespace Application.DTOs.Auth;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// DTO para registro de nueva organización con usuario administrador
/// </summary>
public class RegisterOrganizationRequest
{
    // Datos de la Organización
    [Required(ErrorMessage = "El nombre de la organización es requerido")]
    [MaxLength(160)]
    public string OrganizationName { get; set; } = null!;

    [MaxLength(200)]
    public string? LegalName { get; set; }

    [MaxLength(30)]
    public string? Nit { get; set; }

    // Datos del Usuario Administrador
    [Required(ErrorMessage = "El nombre del administrador es requerido")]
    [MaxLength(80)]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "El apellido del administrador es requerido")]
    [MaxLength(80)]
    public string LastName { get; set; } = null!;

    [Required(ErrorMessage = "El email del administrador es requerido")]
    [EmailAddress]
    [MaxLength(254)]
    public string Email { get; set; } = null!;

    [MaxLength(30)]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "La contraseña es requerida")]
    [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
    public string Password { get; set; } = null!;
}

/// <summary>
/// Respuesta exitosa de registro
/// </summary>
public class RegisterOrganizationResponse
{
    public Guid OrganizationId { get; set; }
    public Guid AdminUserId { get; set; }
    public string Message { get; set; } = "Organización registrada exitosamente";
}
