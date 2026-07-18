namespace Domain.Entities.Nomina;

using Domain.Common;
using Domain.Enums;

/// <summary>
/// Compensación mensual de un asociado (CTA) en el sector solidario
/// </summary>
public class Compensacion : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid AsociadoId { get; set; }
    public Guid OrganizationId { get; set; }

    /// <summary>Período de compensación (mes/año)</summary>
    public DateTime Periodo { get; set; }

    /// <summary>Modelo de cálculo aplicado</summary>
    public CompensacionModelo Modelo { get; set; }

    /// <summary>Valor base antes de cálculos</summary>
    public decimal ValorBase { get; set; }

    /// <summary>Valor calculado final</summary>
    public decimal ValorCalculado { get; set; }

    /// <summary>Observaciones o notas</summary>
    public string? Observaciones { get; set; }
}
