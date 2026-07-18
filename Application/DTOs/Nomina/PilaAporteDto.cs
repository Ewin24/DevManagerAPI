namespace Application.DTOs.Nomina;

using Domain.Enums;

/// <summary>
/// DTO de aporte PILA para asociados tipo CTA
/// </summary>
public record PilaAporteDto
{
    public Guid Id { get; init; }
    public Guid AsociadoId { get; init; }
    public Guid OrganizationId { get; init; }
    public DateTime Periodo { get; init; }
    public PilaTipoAportante TipoAportante { get; init; }
    public decimal IngresoBase { get; init; }
    public decimal AporteEPS { get; init; }
    public decimal AportePension { get; init; }
    public decimal AporteARL { get; init; }
    public decimal Total { get; init; }
}
