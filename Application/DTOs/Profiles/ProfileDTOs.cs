namespace Application.DTOs.Profiles;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// DTO para obtener/actualizar perfil de empleado
/// </summary>
public class EmployeeProfileDto
{
    public Guid UserId { get; set; }

    [MaxLength(800)]
    public string? Bio { get; set; }

    [Range(0, 50, ErrorMessage = "Los años de experiencia deben estar entre 0 y 50")]
    public int? YearsExperience { get; set; }

    [Url(ErrorMessage = "URL de LinkedIn inválida")]
    [MaxLength(300)]
    public string? LinkedInUrl { get; set; }

    [Url(ErrorMessage = "URL de portafolio inválida")]
    [MaxLength(300)]
    public string? PortfolioUrl { get; set; }
}

/// <summary>
/// DTO para actualizar perfil
/// </summary>
public class UpdateProfileRequest
{
    [MaxLength(800)]
    public string? Bio { get; set; }

    [Range(0, 50)]
    public int? YearsExperience { get; set; }

    [Url]
    [MaxLength(300)]
    public string? LinkedInUrl { get; set; }

    [Url]
    [MaxLength(300)]
    public string? PortfolioUrl { get; set; }
}
