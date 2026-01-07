namespace Application.Services;

using Application.Common.Exceptions;
using Application.DTOs.Applications;
using Application.Interfaces;
using Domain.Entities.Projects;
using Domain.Enums;
using Domain.Interfaces.Repositories;

/// <summary>
/// Servicio para gestión de postulaciones a proyectos
/// </summary>
public class ApplicationService : IApplicationService
{
    private readonly IApplicationRepository _applicationRepository;
    private readonly IProjectRepository _projectRepository;

    public ApplicationService(
        IApplicationRepository applicationRepository,
        IProjectRepository projectRepository)
    {
        _applicationRepository = applicationRepository;
        _projectRepository = projectRepository;
    }

    public async Task<Guid> ApplyToProjectAsync(Guid projectId, Guid userId, Guid organizationId, string? message)
    {
        // Verificar que el proyecto existe
        var project = await _projectRepository.GetByIdAsync(projectId, organizationId);
        if (project == null)
        {
            throw new NotFoundException($"Proyecto con ID {projectId} no encontrado");
        }

        // Verificar que el usuario no haya aplicado previamente
        var hasApplied = await _applicationRepository.HasUserAppliedAsync(projectId, userId, organizationId);
        if (hasApplied)
        {
            throw new BusinessValidationException("Ya has aplicado a este proyecto");
        }

        var application = new ProjectApplication
        {
            ProjectId = projectId,
            UserId = userId,
            Message = message,
            Status = ApplicationStatus.Applied,
            OrganizationId = organizationId
        };

        return await _applicationRepository.CreateAsync(application);
    }

    public async Task<bool> ReviewApplicationAsync(
        Guid applicationId,
        Guid organizationId,
        ApplicationStatus status,
        Guid reviewedByUserId,
        string? reviewNotes)
    {
        // Verificar que la aplicación existe
        var application = await _applicationRepository.GetByIdAsync(applicationId, organizationId);
        if (application == null)
        {
            throw new NotFoundException($"Postulación con ID {applicationId} no encontrada");
        }

        // Solo se pueden revisar aplicaciones pendientes
        if (application.Status != ApplicationStatus.Applied)
        {
            throw new BusinessValidationException("Solo se pueden revisar postulaciones pendientes");
        }

        return await _applicationRepository.UpdateStatusAsync(
            applicationId,
            organizationId,
            status,
            reviewedByUserId,
            reviewNotes);
    }

    public async Task<IEnumerable<ApplicationResponse>> GetProjectApplicationsAsync(Guid projectId, Guid organizationId)
    {
        var applications = await _applicationRepository.GetByProjectIdAsync(projectId, organizationId);

        return applications.Select(a => new ApplicationResponse
        {
            Id = a.Id,
            ProjectId = a.ProjectId,
            ProjectName = a.ProjectName,
            UserId = a.UserId,
            UserName = a.UserName,
            Message = a.Message,
            Status = a.Status,
            ReviewNotes = a.ReviewNotes,
            ReviewedAt = a.ReviewedAt,
            CreatedAt = a.CreatedAt
        });
    }
}
