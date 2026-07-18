namespace Application.Interfaces;

using Application.DTOs.Applications;
using Application.DTOs.Assignments;
using Application.DTOs.Projects;
using Domain.Enums;

/// <summary>
/// Servicio para gestión de proyectos
/// </summary>
public interface IProjectService
{
    Task<IEnumerable<ProjectResponse>> GetAllProjectsAsync(Guid organizationId, ProjectStatus? status = null);
    Task<ProjectResponse?> GetProjectByIdAsync(Guid id, Guid organizationId);
    Task<Guid> CreateProjectAsync(CreateProjectRequest request, Guid organizationId);
    Task<ProjectResponse> UpdateProjectAsync(Guid id, UpdateProjectRequest request, Guid organizationId);
    Task<Guid> AddSkillRequirementAsync(Guid projectId, Guid organizationId, AddSkillRequirementRequest request);
    Task<IEnumerable<SkillRequirementResponse>> GetSkillRequirementsAsync(Guid projectId, Guid organizationId);

    // Métodos para el agente
    Task<ProjectDetailsDto?> GetProjectDetailsAsync(Guid projectId, Guid organizationId);
}

/// <summary>
/// Servicio para gestión de postulaciones a proyectos
/// </summary>
public interface IApplicationService
{
    Task<Guid> ApplyToProjectAsync(Guid projectId, Guid userId, Guid organizationId, string? message);
    Task<bool> ReviewApplicationAsync(Guid applicationId, Guid organizationId, ApplicationStatus status, Guid reviewedByUserId, string? reviewNotes);
    Task<IEnumerable<ApplicationResponse>> GetProjectApplicationsAsync(Guid projectId, Guid organizationId);
}

/// <summary>
/// Servicio para gestión de asignaciones a proyectos
/// </summary>
public interface IAssignmentService
{
    Task<Guid> AssignUserToProjectAsync(CreateAssignmentRequest request, Guid organizationId, Guid assignedByUserId);
}