namespace Application.Interfaces;

using Application.DTOs.Feedback;

/// <summary>
/// Servicio para procesar feedback y ejecutar lógica del Agente Inteligente
/// CRÍTICO: Contiene la lógica de calificación automática
/// </summary>
public interface IFeedbackService
{
    /// <summary>
    /// Procesa el feedback de finalización de proyecto
    /// Ejecuta la lógica del agente para actualizar habilidades
    /// </summary>
    Task<FeedbackResponse> ProcessFeedbackAsync(
        SubmitFeedbackRequest request,
        Guid organizationId,
        Guid submittedByUserId);
}
