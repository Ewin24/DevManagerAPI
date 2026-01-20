namespace Domain.Entities.Talent;

using Domain.Common;
using Domain.Entities.IAM;

/// <summary>
/// Perfil profesional extendido del empleado
/// </summary>
public class EmployeeProfile : AuditableEntity
{
    public Guid UserId { get; set; } // PK y FK
    public Guid OrganizationId { get; set; }
    public string? Bio { get; set; }
    public int? YearsExperience { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? PortfolioUrl { get; set; }

    // Navegación
    public User? User { get; set; }
    public Organization? Organization { get; set; }
    public ICollection<EmployeeSkill>? EmployeeSkills { get; set; }
}