namespace Domain.Entities.Reportes;

using Domain.Common;

/// <summary>
/// Reporte para la Superintendencia de la Economía Solidaria
/// Compila información de Balance Social, asociados y cumplimiento SST
/// </summary>
public class ReporteSupersolidaria : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }

    /// <summary>Período del reporte (trimestre/año)</summary>
    public DateTime Periodo { get; set; }

    /// <summary>Datos del Balance Social en JSON</summary>
    public string? BalanceSocialJson { get; set; }

    /// <summary>Datos de asociados en JSON</summary>
    public string? AsociadosJson { get; set; }

    /// <summary>Datos de cumplimiento normativo en JSON</summary>
    public string? CumplimientoJson { get; set; }

    /// <summary>Tipo de reporte (trimestral, anual)</summary>
    public string TipoReporte { get; set; } = "Trimestral";

    /// <summary>¿El reporte ha sido enviado a Supersolidaria?</summary>
    public bool Enviado { get; set; }

    /// <summary>Fecha de envío (si aplica)</summary>
    public DateTime? FechaEnvio { get; set; }

    /// <summary>Observaciones sobre el reporte</summary>
    public string? Observaciones { get; set; }
}
