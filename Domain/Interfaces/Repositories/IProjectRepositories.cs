namespace Domain.Interfaces.Repositories;

using Domain.Entities.Projects;
using Domain.Enums;

/// <summary>
/// Repositorio para proyectos
/// </summary>
public interface IProjectRepository
{
    /// <summary>
    /// Obtiene todos los proyectos de una organización
    /// </summary>
    Task<IEnumerable<Project>> GetAllAsync(Guid organizationId, ProjectStatus? status = null);

    /// <summary>
    /// Obtiene un proyecto por ID
    /// </summary>
    Task<Project?> GetByIdAsync(Guid id, Guid organizationId);

    /// <summary>
    /// Crea un nuevo proyecto
    /// </summary>
    Task<Guid> CreateAsync(Project project);

    /// <summary>
    /// Actualiza un proyecto existente
    /// </summary>
    Task<bool> UpdateAsync(Project project);

    /// <summary>
    /// Obtiene la complejidad de un proyecto (para lógica del agente)
    /// </summary>
    Task<ProjectComplexity?> GetComplexityAsync(Guid projectId, Guid organizationId);

    /// <summary>
    /// Agrega un requisito de habilidad al proyecto
    /// </summary>
    Task<Guid> AddSkillRequirementAsync(ProjectSkillRequirement requirement);

    /// <summary>
    /// Obtiene los requisitos de habilidades de un proyecto
    /// </summary>
    Task<IEnumerable<ProjectSkillRequirement>> GetSkillRequirementsAsync(Guid projectId, Guid organizationId);
}

/// <summary>
/// Repositorio para postulaciones a proyectos
/// </summary>
public interface IApplicationRepository
{
    /// <summary>
    /// Crea una nueva postulación
    /// </summary>
    Task<Guid> CreateAsync(ProjectApplication application);

    /// <summary>
    /// Obtiene una postulación por ID
    /// </summary>
    Task<ProjectApplication?> GetByIdAsync(Guid id, Guid organizationId);

    /// <summary>
    /// Verifica si un usuario ya aplicó a un proyecto
    /// </summary>
    Task<bool> HasUserAppliedAsync(Guid projectId, Guid userId, Guid organizationId);

    /// <summary>
    /// Actualiza el estado de una postulación (aprobar/rechazar)
    /// </summary>
    Task<bool> UpdateStatusAsync(
        Guid id,
        Guid organizationId,
        ApplicationStatus status,
        Guid reviewedByUserId,
        string? reviewNotes);

    /// <summary>
    /// Obtiene postulaciones de un proyecto
    /// </summary>
    Task<IEnumerable<ProjectApplication>> GetByProjectIdAsync(Guid projectId, Guid organizationId);
}

/// <summary>
/// Repositorio para asignaciones a proyectos
/// </summary>
public interface IAssignmentRepository
{
    /// <summary>
    /// Asigna un usuario a un proyecto
    /// </summary>
    Task<Guid> AssignUserAsync(ProjectAssignment assignment);

    /// <summary>
    /// Obtiene una asignación por ID
    /// </summary>
    Task<ProjectAssignment?> GetByIdAsync(Guid id, Guid organizationId);

    /// <summary>
    /// Verifica si un usuario ya está asignado activamente a un proyecto
    /// </summary>
    Task<bool> HasActiveAssignmentAsync(Guid projectId, Guid userId, Guid organizationId);

    /// <summary>
    /// Termina una asignación y crea registro de participación
    /// CRÍTICO: Operación transaccional
    /// </summary>
    Task<Guid> TerminateAssignmentAsync(
        Guid assignmentId,
        Guid organizationId,
        byte contributionScore,
        string? feedbackComments);

    /// <summary>
    /// Obtiene asignaciones activas de un proyecto
    /// </summary>
    Task<IEnumerable<ProjectAssignment>> GetByProjectIdAsync(Guid projectId, Guid organizationId);
}

/// <summary>
/// Repositorio para historial de participación en proyectos
/// </summary>
public interface IParticipationRepository
{
    /// <summary>
    /// Guarda el historial de participación con feedback
    /// </summary>
    Task<Guid> CreateAsync(ProjectParticipation participation);

    /// <summary>
    /// Obtiene el historial de participación de un usuario
    /// </summary>
    Task<IEnumerable<ProjectParticipation>> GetByUserIdAsync(Guid userId, Guid organizationId);
}