namespace Application.DTOs.Config;

/// <summary>
/// DTO genérico para ítems de catálogo/configuración.
/// Representa cualquier tabla paramétrica del esquema config.
/// </summary>
public class ConfigItemDto
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public byte DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// DTO para estados de proyecto (config.ProjectStatuses)
/// </summary>
public class ProjectStatusDto : ConfigItemDto
{
    public bool AllowsApplications { get; set; }
}

/// <summary>
/// DTO para niveles de complejidad (config.ProjectComplexityLevels)
/// </summary>
public class ProjectComplexityLevelDto : ConfigItemDto
{
    public decimal ExperienceMultiplier { get; set; }
}

/// <summary>
/// DTO para estados de postulación (config.ApplicationStatuses)
/// </summary>
public class ApplicationStatusDto : ConfigItemDto
{
    public bool RequiresReviewNotes { get; set; }
    public bool IsFinalState { get; set; }
}

/// <summary>
/// DTO para estados de asignación (config.AssignmentStatuses)
/// </summary>
public class AssignmentStatusDto : ConfigItemDto
{
    public bool IsFinalState { get; set; }
}

/// <summary>
/// DTO para niveles de dominio de habilidad (config.SkillLevels)
/// </summary>
public class SkillLevelDto : ConfigItemDto
{
    public byte? MinYearsExperience { get; set; }
}

/// <summary>
/// DTO para puntajes de contribución (config.ContributionScores)
/// </summary>
public class ContributionScoreDto : ConfigItemDto
{
    public decimal ExperienceBonus { get; set; }
}

/// <summary>
/// DTO para fuentes de evaluación (config.EvaluationSources)
/// </summary>
public class EvaluationSourceDto : ConfigItemDto
{
    public bool IsAutomated { get; set; }
}

/// <summary>
/// DTO para tipos de habilidad (config.SkillTypes)
/// </summary>
public class SkillTypeDto : ConfigItemDto { }

/// <summary>
/// DTO para categorías de habilidad (config.SkillCategories)
/// </summary>
public class SkillCategoryDto : ConfigItemDto
{
    public int? ParentCategoryId { get; set; }
    public string? ParentCategoryName { get; set; }
}

/// <summary>
/// DTO para tipos de acciones del agente (config.AgentActionTypes)
/// </summary>
public class AgentActionTypeDto : ConfigItemDto
{
    public bool RequiresApproval { get; set; }
}

/// <summary>
/// DTO para estados de acciones del agente (config.AgentActionStatuses)
/// </summary>
public class AgentActionStatusDto : ConfigItemDto
{
    public bool IsFinalState { get; set; }
}

/// <summary>
/// DTO para niveles de seniority (config.SeniorityLevels)
/// </summary>
public class SeniorityLevelDto : ConfigItemDto
{
    public byte MinYearsExperience { get; set; }
    public byte? MaxYearsExperience { get; set; }
}

/// <summary>
/// DTO que agrupa todos los catálogos en una sola respuesta
/// </summary>
public class AllConfigCatalogsDto
{
    public IEnumerable<ProjectStatusDto> ProjectStatuses { get; set; } = [];
    public IEnumerable<ProjectComplexityLevelDto> ComplexityLevels { get; set; } = [];
    public IEnumerable<ApplicationStatusDto> ApplicationStatuses { get; set; } = [];
    public IEnumerable<AssignmentStatusDto> AssignmentStatuses { get; set; } = [];
    public IEnumerable<SkillLevelDto> SkillLevels { get; set; } = [];
    public IEnumerable<ContributionScoreDto> ContributionScores { get; set; } = [];
    public IEnumerable<EvaluationSourceDto> EvaluationSources { get; set; } = [];
    public IEnumerable<SkillTypeDto> SkillTypes { get; set; } = [];
    public IEnumerable<SkillCategoryDto> SkillCategories { get; set; } = [];
    public IEnumerable<AgentActionTypeDto> AgentActionTypes { get; set; } = [];
    public IEnumerable<AgentActionStatusDto> AgentActionStatuses { get; set; } = [];
    public IEnumerable<SeniorityLevelDto> SeniorityLevels { get; set; } = [];
}