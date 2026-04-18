namespace Application.Interfaces;

using Application.DTOs.Reports;

/// <summary>
/// Interfaz para el servicio de reportes y análisis estadísticos
/// </summary>
public interface IReportsService
{
    /// <summary>
    /// Obtiene la distribución de habilidades por nivel en la organización
    /// </summary>
    /// <param name="organizationId">ID de la organización</param>
    /// <returns>Enumerable de distribuciones de habilidades</returns>
    Task<IEnumerable<SkillDistributionResponse>> GetSkillsDistributionAsync(Guid organizationId);

    /// <summary>
    /// Obtiene métricas clave de proyectos y habilidades requeridas
    /// </summary>
    /// <param name="organizationId">ID de la organización</param>
    /// <returns>Métricas de proyectos activos</returns>
    Task<ProjectMetricsResponse> GetProjectMetricsAsync(Guid organizationId);

    /// <summary>
    /// Genera un resumen ejecutivo mediante IA basado en el análisis de la organización
    /// </summary>
    /// <param name="organizationId">ID de la organización</param>
    /// <returns>Resumen en formato Markdown</returns>
    Task<AiSummaryResponse> GetAiSummaryAsync(Guid organizationId, Guid userId);

    /// <summary>
    /// Obtiene el estado de utilización de los empleados en la organización
    /// </summary>
    /// <param name="organizationId">ID de la organización</param>
    /// <returns>Detalle de utilización de empleados</returns>
    Task<EmployeeUtilizationResponse> GetEmployeeUtilizationAsync(Guid organizationId);

    /// <summary>
    /// Obtiene métricas estadísticas generales del departamento/organización
    /// </summary>
    /// <param name="organizationId">ID de la organización</param>
    /// <returns>Métricas departamentales</returns>
    Task<DepartmentMetricsResponse> GetDepartmentMetricsAsync(Guid organizationId);
}
