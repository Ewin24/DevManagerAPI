namespace Infrastructure.Repositories;

using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Domain.Enums;
using EfEntities = Infrastructure.Data.Entities;
using DomainEntities = Domain.Entities.Projects;

/// <summary>
/// Repositorio para proyectos usando EF Core
/// </summary>
public class ProjectRepository : Domain.Interfaces.Repositories.IProjectRepository
{
    private readonly DevManagerDbContext _context;

    public ProjectRepository(DevManagerDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<DomainEntities.Project>> GetAllAsync(Guid organizationId, ProjectStatus? status = null)
    {
        var query = _context.Projects
            .AsNoTracking()
            .Where(p => p.OrganizationId == organizationId && !p.IsDeleted);

        if (status.HasValue)
        {
            query = query.Where(p => p.Status == (byte)status.Value);
        }

        var efProjects = await query
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return efProjects.Select(MapToDomain);
    }

    public async Task<DomainEntities.Project?> GetByIdAsync(Guid id, Guid organizationId)
    {
        var efProject = await _context.Projects
            .AsNoTracking()
            .Where(p => p.Id == id && p.OrganizationId == organizationId && !p.IsDeleted)
            .FirstOrDefaultAsync();

        return efProject != null ? MapToDomain(efProject) : null;
    }

    public async Task<Guid> CreateAsync(DomainEntities.Project project)
    {
        var efProject = new EfEntities.Project
        {
            Id = Guid.NewGuid(),
            Code = project.Code,
            Name = project.Name,
            Description = project.Description,
            StartDate = project.StartDate.HasValue ? DateOnly.FromDateTime(project.StartDate.Value) : null,
            EndDate = project.EndDate.HasValue ? DateOnly.FromDateTime(project.EndDate.Value) : null,
            ComplexityLevel = (byte)project.ComplexityLevel,
            Status = (byte)project.Status,
            OrganizationId = project.OrganizationId,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await _context.Projects.AddAsync(efProject);
        await _context.SaveChangesAsync();

        return efProject.Id;
    }

    public async Task<ProjectComplexity?> GetComplexityAsync(Guid projectId, Guid organizationId)
    {
        var project = await _context.Projects
            .AsNoTracking()
            .Where(p => p.Id == projectId && p.OrganizationId == organizationId && !p.IsDeleted)
            .Select(p => p.ComplexityLevel)
            .FirstOrDefaultAsync();

        return project > 0 ? (ProjectComplexity)project : null;
    }

    public async Task<Guid> AddSkillRequirementAsync(DomainEntities.ProjectSkillRequirement requirement)
    {
        var efRequirement = new EfEntities.ProjectSkillRequirement
        {
            Id = Guid.NewGuid(),
            ProjectId = requirement.ProjectId,
            SkillId = requirement.SkillId,
            RequiredLevel = requirement.RequiredLevel,
            IsMandatory = requirement.IsMandatory,
            OrganizationId = requirement.OrganizationId,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await _context.ProjectSkillRequirements.AddAsync(efRequirement);
        await _context.SaveChangesAsync();

        return efRequirement.Id;
    }

    public async Task<IEnumerable<DomainEntities.ProjectSkillRequirement>> GetSkillRequirementsAsync(Guid projectId, Guid organizationId)
    {
        var efRequirements = await _context.ProjectSkillRequirements
            .AsNoTracking()
            .Include(r => r.Skill)
            .Where(r => r.ProjectId == projectId 
                        && r.OrganizationId == organizationId 
                        && !r.IsDeleted)
            .ToListAsync();

        return efRequirements.Select(ef => new DomainEntities.ProjectSkillRequirement
        {
            Id = ef.Id,
            ProjectId = ef.ProjectId,
            SkillId = ef.SkillId,
            SkillName = ef.Skill?.Name ?? string.Empty,
            RequiredLevel = ef.RequiredLevel,
            IsMandatory = ef.IsMandatory,
            OrganizationId = ef.OrganizationId
        });
    }

    private static DomainEntities.Project MapToDomain(EfEntities.Project ef)
    {
        return new DomainEntities.Project
        {
            Id = ef.Id,
            Code = ef.Code,
            Name = ef.Name,
            Description = ef.Description,
            StartDate = ef.StartDate.HasValue ? ef.StartDate.Value.ToDateTime(TimeOnly.MinValue) : null,
            EndDate = ef.EndDate.HasValue ? ef.EndDate.Value.ToDateTime(TimeOnly.MinValue) : null,
            ComplexityLevel = (ProjectComplexity)ef.ComplexityLevel,
            Status = (ProjectStatus)ef.Status,
            OrganizationId = ef.OrganizationId,
            CreatedAt = ef.CreatedAt
        };
    }
}
