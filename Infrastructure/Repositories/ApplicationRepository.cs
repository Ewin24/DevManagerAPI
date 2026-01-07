namespace Infrastructure.Repositories;

using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Domain.Enums;
using EfEntities = Infrastructure.Data.Entities;
using DomainEntities = Domain.Entities.Projects;

/// <summary>
/// Repositorio para postulaciones a proyectos usando EF Core
/// </summary>
public class ApplicationRepository : Domain.Interfaces.Repositories.IApplicationRepository
{
    private readonly DevManagerDbContext _context;

    public ApplicationRepository(DevManagerDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> CreateAsync(DomainEntities.ProjectApplication application)
    {
        var efApplication = new EfEntities.ProjectApplication
        {
            Id = Guid.NewGuid(),
            ProjectId = application.ProjectId,
            UserId = application.UserId,
            Motivation = application.Message,
            Status = (byte)application.Status,
            OrganizationId = application.OrganizationId,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await _context.ProjectApplications.AddAsync(efApplication);
        await _context.SaveChangesAsync();

        return efApplication.Id;
    }

    public async Task<DomainEntities.ProjectApplication?> GetByIdAsync(Guid id, Guid organizationId)
    {
        var efApplication = await _context.ProjectApplications
            .AsNoTracking()
            .Include(a => a.Project)
            .Include(a => a.User)
            .Where(a => a.Id == id && a.OrganizationId == organizationId && !a.IsDeleted)
            .FirstOrDefaultAsync();

        return efApplication != null ? MapToDomain(efApplication) : null;
    }

    public async Task<bool> HasUserAppliedAsync(Guid projectId, Guid userId, Guid organizationId)
    {
        return await _context.ProjectApplications
            .Where(a => a.ProjectId == projectId 
                        && a.UserId == userId
                        && a.OrganizationId == organizationId
                        && !a.IsDeleted)
            .AnyAsync();
    }

    public async Task<bool> UpdateStatusAsync(
        Guid id,
        Guid organizationId,
        ApplicationStatus status,
        Guid reviewedByUserId,
        string? reviewNotes)
    {
        var application = await _context.ProjectApplications
            .Where(a => a.Id == id && a.OrganizationId == organizationId && !a.IsDeleted)
            .FirstOrDefaultAsync();

        if (application == null)
            return false;

        application.Status = (byte)status;
        application.ReviewedByUserId = reviewedByUserId;
        application.ReviewNotes = reviewNotes;
        application.ReviewedAt = DateTime.UtcNow;

        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<IEnumerable<DomainEntities.ProjectApplication>> GetByProjectIdAsync(Guid projectId, Guid organizationId)
    {
        var efApplications = await _context.ProjectApplications
            .AsNoTracking()
            .Include(a => a.User)
            .Where(a => a.ProjectId == projectId 
                        && a.OrganizationId == organizationId 
                        && !a.IsDeleted)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();

        return efApplications.Select(MapToDomain);
    }

    private static DomainEntities.ProjectApplication MapToDomain(EfEntities.ProjectApplication ef)
    {
        return new DomainEntities.ProjectApplication
        {
            Id = ef.Id,
            ProjectId = ef.ProjectId,
            ProjectName = ef.Project?.Name ?? string.Empty,
            UserId = ef.UserId,
            UserName = ef.User?.FirstName != null 
                ? $"{ef.User.FirstName} {ef.User.LastName}" 
                : ef.User?.Email ?? string.Empty,
            Message = ef.Motivation,
            Status = (ApplicationStatus)ef.Status,
            ReviewNotes = ef.ReviewNotes,
            ReviewedAt = ef.ReviewedAt,
            CreatedAt = ef.CreatedAt,
            OrganizationId = ef.OrganizationId
        };
    }
}
