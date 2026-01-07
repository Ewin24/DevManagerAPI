namespace Domain.Entities.Talent;

using Domain.Entities.IAM;
using Domain.Entities.Projects;
using Domain.Enums;

/// <summary>
/// Historial de evaluaciones de habilidades (libro mayor)
/// </summary>
public class SkillEvaluation
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Guid UserId { get; set; }
    public Guid SkillId { get; set; }
    public SkillEvaluationSource Source { get; set; }
    public Guid? ProjectId { get; set; }
    public short DeltaLevel { get; set; } // Cambio en el nivel (+1, -1, etc.)
    public string? Reason { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedByUserId { get; set; }

    // Navegación
    public Organization? Organization { get; set; }
    public User? User { get; set; }
    public Skill? Skill { get; set; }
    public Project? Project { get; set; }
    public User? CreatedBy { get; set; }
}
