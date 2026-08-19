namespace Application.DTOs.Reportes;

/// <summary>
/// DTO para reporte de Supersolidaria
/// Compila Balance Social, Asociados y Cumplimiento SST
/// </summary>
public record ReporteSupersolidariaDto
{
    public Guid Id { get; init; }
    public Guid OrganizationId { get; init; }
    public DateTime Periodo { get; init; }
    public string? BalanceSocialJson { get; init; }
    public string? AsociadosJson { get; init; }
    public string? CumplimientoJson { get; init; }
    public string TipoReporte { get; init; } = null!;
    public bool Enviado { get; init; }
    public DateTime? FechaEnvio { get; init; }
    public string? Observaciones { get; init; }
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// DTO para generar un reporte de Supersolidaria
/// </summary>
public record CreateReporteDto
{
    public Guid OrganizationId { get; init; }
    public DateTime Periodo { get; init; }
    public string TipoReporte { get; init; } = "Trimestral";
}
