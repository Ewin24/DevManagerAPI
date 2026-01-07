namespace Domain.Entities.Talent;

using Domain.Common;
using Domain.Entities.IAM;

/// <summary>
/// Habilidades que posee un empleado con su nivel de competencia
/// </summary>
public class EmployeeSkill : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Guid UserId { get; set; }
    public Guid SkillId { get; set; }
    public byte Level { get; set; } // 1-5 (Novato a Experto)
    public string? EvidenceUrl { get; set; }
    public DateTime? LastValidatedAt { get; set; }
    public Guid? ValidatedByUserId { get; set; } // NULL = validado por sistema/agente
    
    // Propiedades calculadas (usadas por servicios)
    public string SkillName { get; set; } = string.Empty;
    public string? SkillCategory { get; set; }

    // Navegación
    public Organization? Organization { get; set; }
    public User? User { get; set; }
    public Skill? Skill { get; set; }
    public User? ValidatedBy { get; set; }
}
