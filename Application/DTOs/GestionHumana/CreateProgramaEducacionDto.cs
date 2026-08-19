namespace Application.DTOs.GestionHumana;

using Domain.Enums;

/// <summary>
/// DTO de creación de programa educativo cooperativo
/// </summary>
public record CreateProgramaEducacionDto
{
    public Guid OrganizationId { get; init; }
    public string Nombre { get; init; } = null!;
    public string? Descripcion { get; init; }
    public TipoEducacion Tipo { get; init; }
    public int Horas { get; init; }
    public bool EsObligatorio { get; init; }
    public DateTime FechaInicio { get; init; }
    public DateTime? FechaFin { get; init; }
}
