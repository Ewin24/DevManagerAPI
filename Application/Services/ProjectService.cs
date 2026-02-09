namespace Application.Services;

using Application.Common.Exceptions;
using Application.DTOs.Projects;
using Application.Interfaces;
using Domain.Entities.Projects;
using Domain.Enums;
using Domain.Interfaces.Repositories;

/// <summary>
/// Servicio para gestión de proyectos
/// </summary>
public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly ISkillRepository _skillRepository;

    public ProjectService(IProjectRepository projectRepository, ISkillRepository skillRepository)
    {
        _projectRepository = projectRepository;
        _skillRepository = skillRepository;
    }

    public async Task<IEnumerable<ProjectResponse>> GetAllProjectsAsync(Guid organizationId, ProjectStatus? status = null)
    {
        var projects = await _projectRepository.GetAllAsync(organizationId, status);

        return projects.Select(p => new ProjectResponse
        {
            Id = p.Id,
            Code = p.Code,
            Name = p.Name,
            Description = p.Description,
            StartDate = p.StartDate,
            EndDate = p.EndDate,
            ComplexityLevel = p.ComplexityLevel,
            Status = p.Status,
            CreatedAt = p.CreatedAt
        });
    }

    public async Task<ProjectResponse?> GetProjectByIdAsync(Guid id, Guid organizationId)
    {
        var project = await _projectRepository.GetByIdAsync(id, organizationId);

        if (project == null)
            return null;

        return new ProjectResponse
        {
            Id = project.Id,
            Code = project.Code,
            Name = project.Name,
            Description = project.Description,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            ComplexityLevel = project.ComplexityLevel,
            Status = project.Status,
            CreatedAt = project.CreatedAt
        };
    }

    public async Task<Guid> CreateProjectAsync(CreateProjectRequest request, Guid organizationId)
    {
        var project = new Project
        {
            Code = request.Code,
            Name = request.Name,
            Description = request.Description,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            ComplexityLevel = request.ComplexityLevel,
            Status = ProjectStatus.Draft, // Siempre inicia en Draft
            OrganizationId = organizationId
        };

        return await _projectRepository.CreateAsync(project);
    }

    public async Task<ProjectResponse> UpdateProjectAsync(Guid id, UpdateProjectRequest request, Guid organizationId)
    {
        // Obtener el proyecto existente
        var existingProject = await _projectRepository.GetByIdAsync(id, organizationId);

        if (existingProject == null)
        {
            throw new NotFoundException("Proyecto", id);
        }

        // Aplicar cambios (partial update - solo campos proporcionados)
        if (request.Code != null)
            existingProject.Code = request.Code;

        if (request.Name != null)
            existingProject.Name = request.Name;

        if (request.Description != null)
            existingProject.Description = request.Description;

        if (request.StartDate.HasValue)
            existingProject.StartDate = request.StartDate;

        if (request.EndDate.HasValue)
            existingProject.EndDate = request.EndDate;

        if (request.ComplexityLevel.HasValue)
            existingProject.ComplexityLevel = request.ComplexityLevel.Value;

        if (request.Status.HasValue)
            existingProject.Status = request.Status.Value;

        // Guardar cambios
        var updated = await _projectRepository.UpdateAsync(existingProject);

        if (!updated)
        {
            throw new BusinessValidationException("No se pudo actualizar el proyecto");
        }

        // Retornar el proyecto actualizado
        return new ProjectResponse
        {
            Id = existingProject.Id,
            Code = existingProject.Code,
            Name = existingProject.Name,
            Description = existingProject.Description,
            StartDate = existingProject.StartDate,
            EndDate = existingProject.EndDate,
            ComplexityLevel = existingProject.ComplexityLevel,
            Status = existingProject.Status,
            CreatedAt = existingProject.CreatedAt
        };
    }

    public async Task<Guid> AddSkillRequirementAsync(Guid projectId, Guid organizationId, AddSkillRequirementRequest request)
    {
        // Verificar que el proyecto existe
        var project = await _projectRepository.GetByIdAsync(projectId, organizationId);
        if (project == null)
        {
            throw new NotFoundException($"Proyecto con ID {projectId} no encontrado");
        }

        // Verificar que la skill existe
        var skill = await _skillRepository.GetByIdAsync(request.SkillId);
        if (skill == null)
        {
            throw new NotFoundException($"Skill con ID {request.SkillId} no encontrada");
        }

        var requirement = new ProjectSkillRequirement
        {
            ProjectId = projectId,
            SkillId = request.SkillId,
            RequiredLevel = request.RequiredLevel,
            IsMandatory = request.IsMandatory,
            OrganizationId = organizationId
        };

        return await _projectRepository.AddSkillRequirementAsync(requirement);
    }

    public async Task<IEnumerable<SkillRequirementResponse>> GetSkillRequirementsAsync(Guid projectId, Guid organizationId)
    {
        var requirements = await _projectRepository.GetSkillRequirementsAsync(projectId, organizationId);

        return requirements.Select(r => new SkillRequirementResponse
        {
            Id = r.Id,
            SkillId = r.SkillId,
            SkillName = r.SkillName,
            RequiredLevel = r.RequiredLevel,
            IsMandatory = r.IsMandatory
        });
    }

    public async Task<ProjectDetailsDto?> GetProjectDetailsAsync(Guid projectId, Guid organizationId)
    {
        var project = await _projectRepository.GetByIdAsync(projectId, organizationId);

        if (project == null)
            return null;

        var skillRequirements = await GetSkillRequirementsAsync(projectId, organizationId);

        return new ProjectDetailsDto
        {
            Id = project.Id,
            Code = project.Code,
            Name = project.Name,
            Description = project.Description,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            ComplexityLevel = project.ComplexityLevel,
            Status = project.Status,
            SkillRequirements = skillRequirements
        };
    }
}