namespace Application.DTOs.GestionHumana;

using Domain.Enums;

/// <summary>
/// DTO para programa de educación cooperativa
/// </summary>
public record ProgramaEducacionDto
{
    public Guid Id { get; init; }
    public Guid OrganizationId { get; init; }
    public string Nombre { get; init; } = null!;
    public string? Descripcion { get; init; }
    public TipoEducacion Tipo { get; init; }
    public int Horas { get; init; }
    public bool EsObligatorio { get; init; }
    public DateTime FechaInicio { get; init; }
    public DateTime? FechaFin { get; init; }
    public bool Activo { get; init; }
    public DateTime CreatedAt { get; init; }
}
