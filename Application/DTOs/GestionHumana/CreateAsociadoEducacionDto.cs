namespace Application.DTOs.GestionHumana;

/// <summary>
/// DTO de inscripción de un asociado a un programa educativo
/// </summary>
public record CreateAsociadoEducacionDto
{
    public Guid AsociadoId { get; init; }
    public Guid ProgramaEducacionId { get; init; }
    public Guid OrganizationId { get; init; }
}
