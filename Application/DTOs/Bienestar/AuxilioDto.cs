namespace Application.DTOs.Bienestar;

using Domain.Enums;

/// <summary>
/// DTO de auxilio entregado a un asociado
/// </summary>
public record AuxilioDto
{
    public Guid Id { get; init; }
    public Guid AsociadoId { get; init; }
    public Guid OrganizationId { get; init; }
    public Guid? SolicitudBienestarId { get; init; }
    public TipoAuxilio Tipo { get; init; }
    public string TipoNombre { get; init; } = null!;
    public decimal Monto { get; init; }
    public DateTime FechaEntrega { get; init; }
    public string Concepto { get; init; } = null!;
    public bool RequiereReintegro { get; init; }
    public DateTime? FechaLimiteReintegro { get; init; }
}
