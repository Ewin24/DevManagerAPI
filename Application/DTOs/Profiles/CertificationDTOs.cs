namespace Application.DTOs.Profiles;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// DTO de certificación para mostrar
/// </summary>
public class CertificationDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = null!;
    public string? Issuer { get; set; }
    public DateTime? IssueDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string? EvidenceUrl { get; set; }
}

/// <summary>
/// Solicitud para crear/actualizar certificación
/// </summary>
public class CertificationRequest
{
    [Required]
    [MaxLength(160)]
    public string Name { get; set; } = null!;

    [MaxLength(120)]
    public string? Issuer { get; set; }

    public DateTime? IssueDate { get; set; }
    public DateTime? ExpirationDate { get; set; }

    [Url]
    [MaxLength(400)]
    public string? EvidenceUrl { get; set; }
}