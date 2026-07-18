namespace Infrastructure.Repositories;

using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using DomainEntities = Domain.Entities.Projects;
using EfEntities = Infrastructure.Data.Entities;

/// <summary>
/// Repositorio para asignaciones a proyectos usando EF Core
/// </summary>
public class AssignmentRepository : Domain.Interfaces.Repositories.IAssignmentRepository
{
    private readonly DevManagerDbContext _context;

    public AssignmentRepository(DevManagerDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> AssignUserAsync(DomainEntities.ProjectAssignment assignment)
    {
        var efAssignment = new EfEntities.ProjectAssignment
        {
            Id = Guid.NewGuid(),
            ProjectId = assignment.ProjectId,
            UserId = assignment.UserId,
            ProjectRoleId = assignment.ProjectRoleId,
            AssignedByUserId = assignment.AssignedByUserId,
            Status = (byte)assignment.Status,
            OrganizationId = assignment.OrganizationId,
            AssignedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await _context.ProjectAssignments.AddAsync(efAssignment);
        await _context.SaveChangesAsync();

        return efAssignment.Id;
    }

    public async Task<DomainEntities.ProjectAssignment?> GetByIdAsync(Guid id, Guid organizationId)
    {
        var efAssignment = await _context.ProjectAssignments
            .AsNoTracking()
            .Include(a => a.Project)
            .Include(a => a.User)
            .Include(a => a.ProjectRole)
            .Where(a => a.Id == id && a.OrganizationId == organizationId && !a.IsDeleted)
            .FirstOrDefaultAsync();

        return efAssignment != null ? MapToDomain(efAssignment) : null;
    }

    public async Task<bool> HasActiveAssignmentAsync(Guid projectId, Guid userId, Guid organizationId)
    {
        return await _context.ProjectAssignments
            .Where(a => a.ProjectId == projectId
                        && a.UserId == userId
                        && a.OrganizationId == organizationId
                        && a.Status != (byte)AssignmentStatus.Completed
                        && !a.IsDeleted)
            .AnyAsync();
    }

    public async Task<Guid> TerminateAssignmentAsync(
        Guid assignmentId,
        Guid organizationId,
        byte contributionScore,
        string? feedbackComments)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // 1. Actualizar asignación
            var assignment = await _context.ProjectAssignments
                .Include(a => a.ProjectRole)
                .Where(a => a.Id == assignmentId && a.OrganizationId == organizationId)
                .FirstOrDefaultAsync();

            if (assignment == null)
                throw new InvalidOperationException("Assignment not found");

            assignment.Status = (byte)AssignmentStatus.Completed;
            assignment.EndedAt = DateTime.UtcNow;

            // 2. Crear registro de participación
            var participation = new EfEntities.ProjectParticipation
            {
                Id = Guid.NewGuid(),
                ProjectId = assignment.ProjectId,
                UserId = assignment.UserId,
                RoleName = assignment.ProjectRole?.Name,
                ContributionScore = contributionScore,
                FeedbackComments = feedbackComments,
                CompletedAt = DateTime.UtcNow,
                OrganizationId = organizationId
            };

            await _context.ProjectParticipations.AddAsync(participation);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return participation.Id;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<IEnumerable<DomainEntities.ProjectAssignment>> GetByProjectIdAsync(Guid projectId, Guid organizationId)
    {
        var efAssignments = await _context.ProjectAssignments
            .AsNoTracking()
            .Include(a => a.Project)
            .Include(a => a.User)
            .Include(a => a.ProjectRole)
            .Where(a => a.ProjectId == projectId
                        && a.OrganizationId == organizationId
                        && !a.IsDeleted)
            .OrderByDescending(a => a.AssignedAt)
            .ToListAsync();

        return efAssignments.Select(MapToDomain);
    }

    private static DomainEntities.ProjectAssignment MapToDomain(EfEntities.ProjectAssignment ef)
    {
        return new DomainEntities.ProjectAssignment
        {
            Id = ef.Id,
            ProjectId = ef.ProjectId,
            UserId = ef.UserId,
            ProjectRoleId = ef.ProjectRoleId,
            AssignedByUserId = ef.AssignedByUserId,
            Status = (AssignmentStatus)ef.Status,
            AssignedAt = ef.AssignedAt,
            EndedAt = ef.EndedAt,
            OrganizationId = ef.OrganizationId
        };
    }
}
