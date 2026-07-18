namespace Domain.Entities.Nomina;

using Domain.Common;
using Domain.Enums;

/// <summary>
/// Aporte al sistema PILA (Planilla Integrada de Liquidación de Aportes)
/// para asociados tipo CTA — Decreto 2150/2017
/// </summary>
public class PilaAporte : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid AsociadoId { get; set; }
    public Guid OrganizationId { get; set; }

    /// <summary>Período del aporte (mes/año)</summary>
    public DateTime Periodo { get; set; }

    /// <summary>Tipo de aportante (51 = Independiente CTA)</summary>
    public PilaTipoAportante TipoAportante { get; set; }

    /// <summary>Ingreso base de cotización</summary>
    public decimal IngresoBase { get; set; }

    /// <summary>Aporte a EPS (12.5%)</summary>
    public decimal AporteEPS { get; set; }

    /// <summary>Aporte a Pensión (16%)</summary>
    public decimal AportePension { get; set; }

    /// <summary>Aporte a ARL (0.522% - 6.96% según riesgo)</summary>
    public decimal AporteARL { get; set; }

    /// <summary>Total aportes (EPS + Pensión + ARL)</summary>
    public decimal Total { get; set; }
}
