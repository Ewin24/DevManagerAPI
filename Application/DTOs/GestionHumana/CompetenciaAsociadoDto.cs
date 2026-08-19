namespace Application.DTOs.GestionHumana;

/// <summary>
/// DTO de competencia cooperativa de un asociado
/// </summary>
public record CompetenciaAsociadoDto
{
    public Guid Id { get; init; }
    public Guid AsociadoId { get; init; }
    public string Competencia { get; init; } = null!;
    public int Nivel { get; init; }
    public bool Disponible { get; init; }
    public DateTime FechaActualizacion { get; init; }
    public string? Observaciones { get; init; }
}
