namespace Application.Interfaces;

using Application.DTOs.Reportes;

/// <summary>
/// Servicio generador de reportes para Supersolidaria
/// Compila Balance Social, estadísticas de asociados y cumplimiento SST
/// </summary>
public interface IReportGeneratorService
{
    /// <summary>Genera un reporte integral para Supersolidaria</summary>
    Task<ReporteSupersolidariaDto> GenerarReporteAsync(Guid organizationId, DateTime periodo, string tipoReporte);

    /// <summary>Obtiene un reporte existente por período</summary>
    Task<ReporteSupersolidariaDto?> GetReporteByPeriodoAsync(Guid organizationId, DateTime periodo);

    /// <summary>Obtiene todos los reportes de una organización</summary>
    Task<List<ReporteSupersolidariaDto>> GetReportesByOrganizacionAsync(Guid organizationId);

    /// <summary>Marca un reporte como enviado a Supersolidaria</summary>
    Task<ReporteSupersolidariaDto> MarcarEnviadoAsync(Guid reporteId);
}
