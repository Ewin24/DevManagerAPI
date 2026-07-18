namespace Application.Interfaces;

using Application.DTOs.Config;

/// <summary>
/// Servicio para acceso a tablas de configuración/catálogos parametrizables.
/// Provee acceso de solo lectura a las tablas del esquema config.
/// </summary>
public interface IConfigService
{
    /// <summary>
    /// Obtiene todos los catálogos en una sola llamada (ideal para inicialización de frontend)
    /// </summary>
    Task<AllConfigCatalogsDto> GetAllCatalogsAsync();

    /// <summary>
    /// Obtiene los estados de proyecto disponibles
    /// </summary>
    Task<IEnumerable<ProjectStatusDto>> GetProjectStatusesAsync();

    /// <summary>
    /// Obtiene los niveles de complejidad de proyecto
    /// </summary>
    Task<IEnumerable<ProjectComplexityLevelDto>> GetComplexityLevelsAsync();

    /// <summary>
    /// Obtiene los estados de postulación
    /// </summary>
    Task<IEnumerable<ApplicationStatusDto>> GetApplicationStatusesAsync();

    /// <summary>
    /// Obtiene los estados de asignación
    /// </summary>
    Task<IEnumerable<AssignmentStatusDto>> GetAssignmentStatusesAsync();

    /// <summary>
    /// Obtiene los niveles de dominio de habilidades
    /// </summary>
    Task<IEnumerable<SkillLevelDto>> GetSkillLevelsAsync();

    /// <summary>
    /// Obtiene los puntajes de contribución
    /// </summary>
    Task<IEnumerable<ContributionScoreDto>> GetContributionScoresAsync();

    /// <summary>
    /// Obtiene las fuentes de evaluación
    /// </summary>
    Task<IEnumerable<EvaluationSourceDto>> GetEvaluationSourcesAsync();

    /// <summary>
    /// Obtiene los tipos de habilidades
    /// </summary>
    Task<IEnumerable<SkillTypeDto>> GetSkillTypesAsync();

    /// <summary>
    /// Obtiene las categorías de habilidades
    /// </summary>
    Task<IEnumerable<SkillCategoryDto>> GetSkillCategoriesAsync();

    /// <summary>
    /// Obtiene los tipos de acciones del agente
    /// </summary>
    Task<IEnumerable<AgentActionTypeDto>> GetAgentActionTypesAsync();

    /// <summary>
    /// Obtiene los estados de acciones del agente
    /// </summary>
    Task<IEnumerable<AgentActionStatusDto>> GetAgentActionStatusesAsync();

    /// <summary>
    /// Obtiene los niveles de seniority
    /// </summary>
    Task<IEnumerable<SeniorityLevelDto>> GetSeniorityLevelsAsync();
}