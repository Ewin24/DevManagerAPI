namespace Application.DTOs.Auth;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// DTO para login de usuario
/// </summary>
public class LoginRequest
{
    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "El formato del email es inválido")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "La contraseña es requerida")]
    [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
    public string Password { get; set; } = null!;
}

/// <summary>
/// Respuesta exitosa de autenticación con token JWT
/// </summary>
public class LoginResponse
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public Guid OrganizationId { get; set; }
    public string Token { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
}
