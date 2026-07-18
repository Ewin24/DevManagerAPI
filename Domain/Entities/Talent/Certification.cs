namespace Domain.Entities.Talent;

using Domain.Common;
using Domain.Entities.IAM;

/// <summary>
/// Certificaciones oficiales del empleado
/// </summary>
public class Certification : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = null!;
    public string? Issuer { get; set; }
    public DateTime? IssueDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string? EvidenceUrl { get; set; }

    // Navegación
    public Organization? Organization { get; set; }
    public User? User { get; set; }
}
