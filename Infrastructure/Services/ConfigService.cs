namespace Infrastructure.Services;

using Application.DTOs.Config;
using Application.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Implementación del servicio de configuración/catálogos.
/// Accede directamente al DbContext porque los catálogos son tablas de solo lectura
/// sin lógica de negocio (patrón similar a TokenService).
/// </summary>
public class ConfigService : IConfigService
{
    private readonly DevManagerDbContext _context;

    public ConfigService(DevManagerDbContext context)
    {
        _context = context;
    }

    public async Task<AllConfigCatalogsDto> GetAllCatalogsAsync()
    {
        return new AllConfigCatalogsDto
        {
            ProjectStatuses = await GetProjectStatusesAsync(),
            ComplexityLevels = await GetComplexityLevelsAsync(),
            ApplicationStatuses = await GetApplicationStatusesAsync(),
            AssignmentStatuses = await GetAssignmentStatusesAsync(),
            SkillLevels = await GetSkillLevelsAsync(),
            ContributionScores = await GetContributionScoresAsync(),
            EvaluationSources = await GetEvaluationSourcesAsync(),
            SkillTypes = await GetSkillTypesAsync(),
            SkillCategories = await GetSkillCategoriesAsync(),
            AgentActionTypes = await GetAgentActionTypesAsync(),
            AgentActionStatuses = await GetAgentActionStatusesAsync(),
            SeniorityLevels = await GetSeniorityLevelsAsync()
        };
    }

    public async Task<IEnumerable<ProjectStatusDto>> GetProjectStatusesAsync()
    {
        return await _context.ProjectStatuses
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.DisplayOrder)
            .Select(x => new ProjectStatusDto
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Description = x.Description,
                DisplayOrder = x.DisplayOrder,
                IsActive = x.IsActive,
                AllowsApplications = x.AllowsApplications
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<ProjectComplexityLevelDto>> GetComplexityLevelsAsync()
    {
        return await _context.ProjectComplexityLevels
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.DisplayOrder)
            .Select(x => new ProjectComplexityLevelDto
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Description = x.Description,
                DisplayOrder = x.DisplayOrder,
                IsActive = x.IsActive,
                ExperienceMultiplier = x.ExperienceMultiplier
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<ApplicationStatusDto>> GetApplicationStatusesAsync()
    {
        return await _context.ApplicationStatuses
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.DisplayOrder)
            .Select(x => new ApplicationStatusDto
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Description = x.Description,
                DisplayOrder = x.DisplayOrder,
                IsActive = x.IsActive,
                RequiresReviewNotes = x.RequiresReviewNotes,
                IsFinalState = x.IsFinalState
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<AssignmentStatusDto>> GetAssignmentStatusesAsync()
    {
        return await _context.AssignmentStatuses
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.DisplayOrder)
            .Select(x => new AssignmentStatusDto
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Description = x.Description,
                DisplayOrder = x.DisplayOrder,
                IsActive = x.IsActive,
                IsFinalState = x.IsFinalState
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<SkillLevelDto>> GetSkillLevelsAsync()
    {
        return await _context.SkillLevels
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.DisplayOrder)
            .Select(x => new SkillLevelDto
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Description = x.Description,
                DisplayOrder = x.DisplayOrder,
                IsActive = x.IsActive,
                MinYearsExperience = x.MinYearsExperience
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<ContributionScoreDto>> GetContributionScoresAsync()
    {
        return await _context.ContributionScores
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.DisplayOrder)
            .Select(x => new ContributionScoreDto
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Description = x.Description,
                DisplayOrder = x.DisplayOrder,
                IsActive = x.IsActive,
                ExperienceBonus = x.ExperienceBonus
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<EvaluationSourceDto>> GetEvaluationSourcesAsync()
    {
        return await _context.EvaluationSources
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.DisplayOrder)
            .Select(x => new EvaluationSourceDto
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Description = x.Description,
                DisplayOrder = x.DisplayOrder,
                IsActive = x.IsActive,
                IsAutomated = x.IsAutomated
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<SkillTypeDto>> GetSkillTypesAsync()
    {
        return await _context.SkillTypes
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.DisplayOrder)
            .Select(x => new SkillTypeDto
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Description = x.Description,
                DisplayOrder = x.DisplayOrder,
                IsActive = x.IsActive
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<SkillCategoryDto>> GetSkillCategoriesAsync()
    {
        return await _context.SkillCategories
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.DisplayOrder)
            .Select(x => new SkillCategoryDto
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Description = x.Description,
                DisplayOrder = x.DisplayOrder,
                IsActive = x.IsActive,
                ParentCategoryId = x.ParentCategoryId,
                ParentCategoryName = x.ParentCategory != null ? x.ParentCategory.Name : null
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<AgentActionTypeDto>> GetAgentActionTypesAsync()
    {
        return await _context.AgentActionTypes
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.DisplayOrder)
            .Select(x => new AgentActionTypeDto
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Description = x.Description,
                DisplayOrder = x.DisplayOrder,
                IsActive = x.IsActive,
                RequiresApproval = x.RequiresApproval
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<AgentActionStatusDto>> GetAgentActionStatusesAsync()
    {
        return await _context.AgentActionStatuses
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.DisplayOrder)
            .Select(x => new AgentActionStatusDto
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Description = x.Description,
                DisplayOrder = x.DisplayOrder,
                IsActive = x.IsActive,
                IsFinalState = x.IsFinalState
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<SeniorityLevelDto>> GetSeniorityLevelsAsync()
    {
        return await _context.SeniorityLevels
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.DisplayOrder)
            .Select(x => new SeniorityLevelDto
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Description = x.Description,
                DisplayOrder = x.DisplayOrder,
                IsActive = x.IsActive,
                MinYearsExperience = x.MinYearsExperience,
                MaxYearsExperience = x.MaxYearsExperience
            })
            .ToListAsync();
    }
}