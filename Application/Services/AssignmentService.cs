namespace Application.Services;

using Application.Common.Exceptions;
using Application.DTOs.Assignments;
using Application.Interfaces;
using Domain.Entities.Projects;
using Domain.Enums;
using Domain.Interfaces.Repositories;

/// <summary>
/// Servicio para gestión de asignaciones a proyectos
/// </summary>
public class AssignmentService : IAssignmentService
{
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly IApplicationRepository _applicationRepository;
    private readonly IProjectRepository _projectRepository;

    public AssignmentService(
        IAssignmentRepository assignmentRepository,
        IApplicationRepository applicationRepository,
        IProjectRepository projectRepository)
    {
        _assignmentRepository = assignmentRepository;
        _applicationRepository = applicationRepository;
        _projectRepository = projectRepository;
    }

    public async Task<Guid> AssignUserToProjectAsync(
        CreateAssignmentRequest request,
        Guid organizationId,
        Guid assignedByUserId)
    {
        // Verificar que el proyecto existe
        var project = await _projectRepository.GetByIdAsync(request.ProjectId, organizationId);
        if (project == null)
        {
            throw new NotFoundException($"Proyecto con ID {request.ProjectId} no encontrado");
        }

        // Verificar que el usuario no tenga una asignación activa en el proyecto
        var hasActiveAssignment = await _assignmentRepository.HasActiveAssignmentAsync(
            request.ProjectId,
            request.UserId,
            organizationId);

        if (hasActiveAssignment)
        {
            throw new BusinessValidationException("El usuario ya tiene una asignación activa en este proyecto");
        }

        // Si hay una aplicación previa, actualizarla a "Hired"
        if (request.ApplicationId.HasValue)
        {
            await _applicationRepository.UpdateStatusAsync(
                request.ApplicationId.Value,
                organizationId,
                ApplicationStatus.Approved,
                assignedByUserId,
                "Aceptado y asignado al proyecto");
        }

        // Crear la asignación
        var assignment = new ProjectAssignment
        {
            ProjectId = request.ProjectId,
            UserId = request.UserId,
            ProjectRoleId = request.ProjectRoleId,
            AssignedByUserId = assignedByUserId,
            Status = AssignmentStatus.Active,
            OrganizationId = organizationId
        };

        return await _assignmentRepository.AssignUserAsync(assignment);
    }
}
