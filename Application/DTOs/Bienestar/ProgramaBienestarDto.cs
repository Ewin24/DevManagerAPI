namespace Application.DTOs.Bienestar;

/// <summary>
/// DTO para programa de bienestar social
/// </summary>
public record ProgramaBienestarDto
{
    public Guid Id { get; init; }
    public Guid OrganizationId { get; init; }
    public string Nombre { get; init; } = null!;
    public string? Descripcion { get; init; }
    public decimal Presupuesto { get; init; }
    public DateTime FechaInicio { get; init; }
    public DateTime? FechaFin { get; init; }
    public bool Activo { get; init; }
    public int? MaxBeneficiarios { get; init; }
    public DateTime CreatedAt { get; init; }
}
