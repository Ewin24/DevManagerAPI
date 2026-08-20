namespace Domain.Interfaces.Repositories;

using Domain.Entities.Reportes;

/// <summary>
/// Repositorio de reportes para Supersolidaria (single-entity: Domain IS the EF entity)
/// </summary>
public interface IReporteSupersolidariaRepository
{
    Task<ReporteSupersolidaria> CreateAsync(ReporteSupersolidaria reporte);

    Task<ReporteSupersolidaria> UpdateAsync(ReporteSupersolidaria reporte);

    Task<ReporteSupersolidaria?> GetByIdAsync(Guid id);

    Task<ReporteSupersolidaria?> GetByOrganizationAndPeriodoAsync(Guid organizationId, DateTime periodo);

    Task<List<ReporteSupersolidaria>> GetByOrganizationAsync(Guid organizationId);
}