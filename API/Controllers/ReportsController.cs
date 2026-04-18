namespace API.Controllers;

using Application.Common.Models;
using Application.DTOs.Reports;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Controller para reportes y análisis estadísticos de la organización
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IReportsService _reportsService;
    private readonly ILogger<ReportsController> _logger;

    public ReportsController(IReportsService reportsService, ILogger<ReportsController> logger)
    {
        _reportsService = reportsService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene la distribución de habilidades por nivel en la organización
    /// </summary>
    /// <remarks>
    /// Retorna un análisis agrupado de todas las habilidades en la organización,
    /// mostrando cuántos empleados tienen cada nivel de competencia para cada habilidad.
    /// 
    /// **Ejemplo de Response (200 OK):**
    /// 
    ///     {
    ///         "success": true,
    ///         "message": null,
    ///         "data": [
    ///             {
    ///                 "skillName": "React",
    ///                 "averageLevel": 3.5,
    ///                 "totalEmployees": 42,
    ///                 "levelDistribution": {
    ///                     "1": 5,
    ///                     "2": 10,
    ///                     "3": 15,
    ///                     "4": 10,
    ///                     "5": 2
    ///                 }
    ///             },
    ///             {
    ///                 "skillName": "C#",
    ///                 "averageLevel": 4.2,
    ///                 "totalEmployees": 28,
    ///                 "levelDistribution": {
    ///                     "1": 0,
    ///                     "2": 5,
    ///                     "3": 12,
    ///                     "4": 10,
    ///                     "5": 1
    ///                 }
    ///             }
    ///         ]
    ///     }
    /// 
    /// **Niveles:**
    /// - 1 = Novato (básico)
    /// - 2 = Principiante
    /// - 3 = Intermedio
    /// - 4 = Avanzado
    /// - 5 = Experto
    /// 
    /// **Casos de uso:**
    /// - Dashboard de capacidades organizacionales
    /// - Planificación de capacitación
    /// - Análisis de fortalezas técnicas
    /// </remarks>
    /// <returns>Distribución de habilidades por nivel</returns>
    /// <response code="200">Distribución de habilidades obtenida exitosamente</response>
    /// <response code="401">No autenticado</response>
    [HttpGet("skills-distribution")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<SkillDistributionResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetSkillsDistribution()
    {
        var organizationId = Guid.Parse(User.FindFirst("OrganizationId")?.Value ?? string.Empty);
        
        _logger.LogInformation("Obteniendo distribución de habilidades para org {OrgId}", organizationId);

        var distribution = await _reportsService.GetSkillsDistributionAsync(organizationId);
        
        return Ok(ApiResponse<IEnumerable<SkillDistributionResponse>>.SuccessResponse(
            distribution,
            "Distribución de habilidades obtenida exitosamente"));
    }

    /// <summary>
    /// Obtiene métricas clave de proyectos activos
    /// </summary>
    /// <remarks>
    /// Retorna un análisis consolidado de los proyectos activos incluyendo:
    /// - Total de proyectos activos en la organización
    /// - Cantidad de proyectos en riesgo (falta de habilidades requeridas)
    /// - Top de habilidades más demandadas en los proyectos
    /// 
    /// **Ejemplo de Response (200 OK):**
    /// 
    ///     {
    ///         "success": true,
    ///         "message": null,
    ///         "data": {
    ///             "totalActiveProjects": 12,
    ///             "projectsAtRisk": 3,
    ///             "mostDemandedSkills": [
    ///                 {
    ///                     "skillName": "C#",
    ///                     "requiredInProjects": 8
    ///                 },
    ///                 {
    ///                     "skillName": "Azure",
    ///                     "requiredInProjects": 6
    ///                 },
    ///                 {
    ///                     "skillName": "React",
    ///                     "requiredInProjects": 5
    ///                 }
    ///             ]
    ///         }
    ///     }
    /// 
    /// **Definiciones:**
    /// - **Proyectos en Riesgo:** Proyectos que tienen requisitos de habilidades obligatorias no cubiertas
    /// - **Habilidades Demandadas:** Conteo de proyectos activos que requieren cada habilidad
    /// 
    /// **Casos de uso:**
    /// - Dashboard ejecutivo de proyectos
    /// - Identificación de brechas de talento críticas
    /// - Planificación de recursos humanos
    /// </remarks>
    /// <returns>Métricas de proyectos</returns>
    /// <response code="200">Métricas de proyectos obtenidas exitosamente</response>
    /// <response code="401">No autenticado</response>
    [HttpGet("project-metrics")]
    [ProducesResponseType(typeof(ApiResponse<ProjectMetricsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetProjectMetrics()
    {
        var organizationId = Guid.Parse(User.FindFirst("OrganizationId")?.Value ?? string.Empty);
        
        _logger.LogInformation("Obteniendo métricas de proyectos para org {OrgId}", organizationId);

        var metrics = await _reportsService.GetProjectMetricsAsync(organizationId);
        
        return Ok(ApiResponse<ProjectMetricsResponse>.SuccessResponse(
            metrics,
            "Métricas de proyectos obtenidas exitosamente"));
    }

    /// <summary>
    /// Obtiene un resumen ejecutivo generado por IA
    /// </summary>
    /// <remarks>
    /// Genera un análisis narrativo en Markdown basado en:
    /// - Habilidades más fuertes en la organización
    /// - Brechas críticas de talento
    /// - Recomendaciones específicas para mejorar capacidades
    /// 
    /// El resumen es generado por el agente de IA analizando los datos reales de la organización.
    /// 
    /// **Ejemplo de Response (200 OK):**
    /// 
    ///     {
    ///         "success": true,
    ///         "message": null,
    ///         "data": {
    ///             "markdown": "### Resumen Ejecutivo - Análisis de Talento\n\nLa organización posee una **fortaleza significativa en Backend (.NET)**, con 28 empleados en niveles avanzado (4) y experto (5). Esta capacidad posiciona favorablemente los 8 proyectos que requieren C# activamente.\n\nSin embargo, existe una **brecha crítica en DevOps y Kubernetes**, donde solo 2 empleados poseen nivel 3, insuficiente para los 3 proyectos que lo requieren como habilidad mandatoria. Se recomienda:\n\n1. **Capacitación inmediata** de 2-3 arquitectos de sistemas en Kubernetes (nivel 4 target)\n2. **Asociación externa** para proyectos críticos en 90 días\n3. **Plan de retención** para los especialistas actuales\n\nLa distribución de React está equilibrada con 42 empleados cubiertos. Nivel promedio: 3.5."
    ///         }
    ///     }
    /// 
    /// **Características del resumen:**
    /// - Formato Markdown con encabezados y listas
    /// - Análisis basado en datos reales de la organización
    /// - Recomendaciones accionables
    /// - Tono profesional y ejecutivo
    /// 
    /// **Casos de uso:**
    /// - Presentaciones ejecutivas
    /// - Planificación estratégica de talento
    /// - Reportes gerenciales
    /// </remarks>
    /// <returns>Resumen ejecutivo en Markdown</returns>
    /// <response code="200">Resumen ejecutivo generado exitosamente</response>
    /// <response code="401">No autenticado</response>
    [HttpGet("ai-summary")]
    [ProducesResponseType(typeof(ApiResponse<AiSummaryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAiSummary()
    {
        var organizationId = Guid.Parse(User.FindFirst("OrganizationId")?.Value ?? string.Empty);
        var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var userId = string.IsNullOrEmpty(userIdStr) ? Guid.Empty : Guid.Parse(userIdStr);
        
        _logger.LogInformation("Generando resumen ejecutivo por IA para org {OrgId}", organizationId);

        var summary = await _reportsService.GetAiSummaryAsync(organizationId, userId);
        
         return Ok(ApiResponse<AiSummaryResponse>.SuccessResponse(
            summary,
            "Resumen ejecutivo generado exitosamente"));
    }

    /// <summary>
    /// Obtiene el estado de utilización de empleados
    /// </summary>
    /// <remarks>
    /// Retorna un análisis detallado de la utilización de cada empleado en la organización:
    /// - Cantidad de proyectos activos asignados por empleado
    /// - Porcentaje de utilización estimado
    /// - Estado de asignación (asignado/sin asignar)
    /// - Resumen agregado de utilización general
    /// 
    /// **Ejemplo de Response (200 OK):**
    /// 
    ///     {
    ///         "success": true,
    ///         "message": null,
    ///         "data": {
    ///             "totalEmployees": 50,
    ///             "allocatedEmployees": 42,
    ///             "unallocatedEmployees": 8,
    ///             "averageProjectsPerEmployee": 1.8,
    ///             "utilizationRate": 84.0,
    ///             "employees": [
    ///                 {
    ///                     "employeeId": "guid",
    ///                     "fullName": "Juan García",
    ///                     "email": "juan@company.com",
    ///                     "activeProjectCount": 2,
    ///                     "assignedProjects": ["Mobile App", "Backend API"],
    ///                     "utilizationPercentage": 50,
    ///                     "allocationStatus": "Asignado",
    ///                     "skillsCount": 8,
    ///                     "yearsExperience": 5
    ///                 },
    ///                 {
    ///                     "employeeId": "guid",
    ///                     "fullName": "María López",
    ///                     "email": "maria@company.com",
    ///                     "activeProjectCount": 0,
    ///                     "assignedProjects": [],
    ///                     "utilizationPercentage": 0,
    ///                     "allocationStatus": "Sin asignar",
    ///                     "skillsCount": 12,
    ///                     "yearsExperience": 8
    ///                 }
    ///             ]
    ///         }
    ///     }
    /// 
    /// **Definiciones:**
    /// - **Utilización:** Basada en cantidad de proyectos activos (cada proyecto ~25%, máx. 100%)
    /// - **Proyectos Activos:** Solo cuenta asignaciones en estado En Progreso
    /// - **Tasa de Utilización:** Porcentaje de empleados con al menos 1 proyecto activo
    /// 
    /// **Casos de uso:**
    /// - Dashboard de capacidad de equipo
    /// - Identificación de empleados disponibles
    /// - Planificación de asignaciones de proyectos
    /// - Análisis de carga de trabajo
    /// </remarks>
    /// <returns>Detalle de utilización de empleados</returns>
    /// <response code="200">Utilización de empleados obtenida exitosamente</response>
    /// <response code="401">No autenticado</response>
    [HttpGet("employee-utilization")]
    [ProducesResponseType(typeof(ApiResponse<EmployeeUtilizationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetEmployeeUtilization()
    {
        var organizationId = Guid.Parse(User.FindFirst("OrganizationId")?.Value ?? string.Empty);
        
        _logger.LogInformation("Obteniendo utilización de empleados para org {OrgId}", organizationId);

        var utilization = await _reportsService.GetEmployeeUtilizationAsync(organizationId);
        
        return Ok(ApiResponse<EmployeeUtilizationResponse>.SuccessResponse(
            utilization,
            "Utilización de empleados obtenida exitosamente"));
    }

    /// <summary>
    /// Obtiene métricas estadísticas del departamento/organización
    /// </summary>
    /// <remarks>
    /// Retorna un análisis consolidado de las métricas estadísticas de la organización:
    /// - Total de empleados y activos
    /// - Promedio de habilidades por empleado
    /// - Distribución por experiencia (Junior, Mid-level, Senior, Lead)
    /// - Total de habilidades únicas
    /// - Empleados con certificaciones validadas
    /// 
    /// **Ejemplo de Response (200 OK):**
    /// 
    ///     {
    ///         "success": true,
    ///         "message": null,
    ///         "data": {
    ///             "totalEmployees": 50,
    ///             "activeEmployees": 48,
    ///             "averageSkillsPerEmployee": 4.2,
    ///             "totalUniqueSkills": 32,
    ///             "employeesWithCertifications": 18,
    ///             "averageYearsExperience": 6.5,
    ///             "totalRoles": 5,
    ///             "experienceDistribution": {
    ///                 "junior": 8,
    ///                 "midLevel": 18,
    ///                 "senior": 16,
    ///                 "lead": 6
    ///             }
    ///         }
    ///     }
    /// 
    /// **Definições:**
    /// - **Junior:** 0-2 años de experiencia
    /// - **Mid-level:** 3-5 años de experiencia
    /// - **Senior:** 6-10 años de experiencia
    /// - **Lead:** 11+ años de experiencia
    /// - **Certificaciones:** Skills validadas por un supervisor
    /// 
    /// **Casos de uso:**
    /// - Dashboard de capacidades organizacionales
    /// - Análisis de madurez técnica del equipo
    /// - Identificación de necesidades de capacitación
    /// - Planificación de sucesión y retención
    /// </remarks>
    /// <returns>Métricas departamentales</returns>
    /// <response code="200">Métricas departamentales obtenidas exitosamente</response>
    /// <response code="401">No autenticado</response>
    [HttpGet("department-metrics")]
    [ProducesResponseType(typeof(ApiResponse<DepartmentMetricsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetDepartmentMetrics()
    {
        var organizationId = Guid.Parse(User.FindFirst("OrganizationId")?.Value ?? string.Empty);
        
        _logger.LogInformation("Obteniendo métricas departamentales para org {OrgId}", organizationId);

        var metrics = await _reportsService.GetDepartmentMetricsAsync(organizationId);
        
        return Ok(ApiResponse<DepartmentMetricsResponse>.SuccessResponse(
            metrics,
            "Métricas departamentales obtenidas exitosamente"));
    }

    /// <summary>
    /// Helper para obtener el ID de la organización del token JWT
    /// </summary>
    private Guid GetOrganizationId() =>
        Guid.Parse(User.FindFirst("OrganizationId")?.Value ?? string.Empty);
}
