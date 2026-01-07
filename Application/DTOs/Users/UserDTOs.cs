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
    public DateTime CreatedAt { get; set; }
}
