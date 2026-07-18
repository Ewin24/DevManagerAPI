namespace Domain.Entities.Projects;

using Domain.Entities.IAM;

/// <summary>
/// Historial permanente de participación en proyectos con feedback
/// </summary>
public class ProjectParticipation
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid UserId { get; set; }
    public string? RoleName { get; set; }
    public byte? ContributionScore { get; set; } // 1-5
    public string? FeedbackComments { get; set; } // Crítico para NLP del agente
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsDeleted { get; set; }

    // Navegación
    public Organization? Organization { get; set; }
    public Project? Project { get; set; }
    public User? User { get; set; }
}
