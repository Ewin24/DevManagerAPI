namespace Application.DTOs.Feedback;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// DTO para enviar feedback al finalizar proyecto
/// CRÍTICO: Dispara lógica del Agente Inteligente
/// </summary>
public class SubmitFeedbackRequest
{
    [Required]
    public Guid AssignmentId { get; set; }

    [Range(1, 5, ErrorMessage = "La calificación debe estar entre 1 y 5")]
    public byte ContributionScore { get; set; }

    [MaxLength(2000)]
    public string? FeedbackComments { get; set; }
}

/// <summary>
/// DTO de respuesta del feedback procesado
/// </summary>
public class FeedbackResponse
{
    public Guid ParticipationId { get; set; }
    public string Message { get; set; } = "Feedback registrado exitosamente";
    public int SkillsUpgraded { get; set; } // Cuántas habilidades subieron de nivel
    public List<SkillUpgradeDetail> UpgradedSkills { get; set; } = new();
}

/// <summary>
/// Detalle de habilidades que subieron de nivel
/// </summary>
public class SkillUpgradeDetail
{
    public string SkillName { get; set; } = null!;
    public byte PreviousLevel { get; set; }
    public byte NewLevel { get; set; }
    public string Reason { get; set; } = null!;
}
