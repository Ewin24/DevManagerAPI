namespace Application.DTOs.GestionHumana;

/// <summary>
/// DTO de inscripción y progreso educativo de un asociado
/// </summary>
public record AsociadoEducacionDto
{
    public Guid Id { get; init; }
    public Guid AsociadoId { get; init; }
    public Guid ProgramaEducacionId { get; init; }
    public string? ProgramaNombre { get; init; }
    public string? TipoEducacion { get; init; }
    public int HorasPrograma { get; init; }
    public int HorasCursadas { get; init; }
    public decimal Progreso { get; init; }
    public DateTime FechaInscripcion { get; init; }
    public DateTime? FechaCompletado { get; init; }
    public bool Completado { get; init; }
    public string? Resultado { get; init; }
}
