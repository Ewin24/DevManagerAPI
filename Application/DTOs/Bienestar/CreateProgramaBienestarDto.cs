namespace Application.DTOs.Bienestar;

/// <summary>
/// DTO de creación de programa de bienestar social
/// </summary>
public record CreateProgramaBienestarDto
{
    public Guid OrganizationId { get; init; }
    public string Nombre { get; init; } = null!;
    public string? Descripcion { get; init; }
    public decimal Presupuesto { get; init; }
    public DateTime FechaInicio { get; init; }
    public DateTime? FechaFin { get; init; }
    public int? MaxBeneficiarios { get; init; }
}
