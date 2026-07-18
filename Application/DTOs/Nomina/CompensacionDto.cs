namespace Application.DTOs.Nomina;

using Domain.Enums;

/// <summary>
/// DTO de compensación para asociados del sector solidario
/// </summary>
public record CompensacionDto
{
    public Guid Id { get; init; }
    public Guid AsociadoId { get; init; }
    public Guid OrganizationId { get; init; }
    public DateTime Periodo { get; init; }
    public CompensacionModelo Modelo { get; init; }
    public decimal ValorBase { get; init; }
    public decimal ValorCalculado { get; init; }
    public string? Observaciones { get; init; }
    public DateTime CreatedAt { get; init; }
}
