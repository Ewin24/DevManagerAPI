namespace Application.DTOs.Nomina;

using Domain.Enums;

/// <summary>
/// DTO de creación de compensación para un asociado
/// </summary>
public record CreateCompensacionDto
{
    public Guid AsociadoId { get; init; }
    public Guid OrganizationId { get; init; }

    /// <summary>Período de compensación (año, mes, día — se usa mes/año)</summary>
    public DateTime Periodo { get; init; }

    /// <summary>Modelo de cálculo</summary>
    public CompensacionModelo Modelo { get; init; }

    /// <summary>Valor base: días o tarifa diaria según modelo</summary>
    public decimal ValorBase { get; init; }

    /// <summary>Observaciones opcionales</summary>
    public string? Observaciones { get; init; }
}
