namespace Infrastructure.Repositories;

using Domain.Enums;
using Domain.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using DomainSolidarity = Domain.Entities.Solidarity;
using EfEntities = Infrastructure.Data.Entities;

/// <summary>
/// Repositorio de educación cooperativa usando EF Core (siempre filtrado por OrganizationId)
/// </summary>
public class EducationRepository : IEducationRepository
{
    private readonly DevManagerDbContext _context;

    public EducationRepository(DevManagerDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<DomainSolidarity.EducationProgram>> GetProgramsAsync(Guid organizationId)
    {
        var programs = await _context.EducationPrograms
            .AsNoTracking()
            .Where(p => p.OrganizationId == organizationId && !p.IsDeleted)
            .OrderByDescending(p => p.StartDate).ThenBy(p => p.Name)
            .ToListAsync();
        return programs.Select(MapProgram);
    }

    public async Task<DomainSolidarity.EducationProgram?> GetProgramByIdAsync(Guid id, Guid organizationId)
    {
        var program = await _context.EducationPrograms
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id && p.OrganizationId == organizationId && !p.IsDeleted);
        return program == null ? null : MapProgram(program);
    }

    public async Task CreateProgramAsync(DomainSolidarity.EducationProgram program)
    {
        _context.EducationPrograms.Add(new EfEntities.EducationProgram
        {
            Id = program.Id,
            OrganizationId = program.OrganizationId,
            Name = program.Name,
            Type = (byte)program.Type,
            RequiredHours = program.RequiredHours,
            StartDate = program.StartDate,
            EndDate = program.EndDate,
            CreatedAt = program.CreatedAt,
            CreatedByUserId = program.CreatedByUserId
        });
        await _context.SaveChangesAsync();
    }

    public async Task UpdateProgramAsync(DomainSolidarity.EducationProgram program)
    {
        var ef = await _context.EducationPrograms
            .FirstOrDefaultAsync(p => p.Id == program.Id && p.OrganizationId == program.OrganizationId && !p.IsDeleted);
        if (ef == null)
            return;

        ef.Name = program.Name;
        ef.Type = (byte)program.Type;
        ef.RequiredHours = program.RequiredHours;
        ef.StartDate = program.StartDate;
        ef.EndDate = program.EndDate;
        ef.UpdatedAt = DateTime.UtcNow;
        ef.UpdatedByUserId = program.UpdatedByUserId;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteProgramAsync(Guid id, Guid organizationId, Guid deletedByUserId)
    {
        var ef = await _context.EducationPrograms
            .FirstOrDefaultAsync(p => p.Id == id && p.OrganizationId == organizationId && !p.IsDeleted);
        if (ef == null)
            return;

        ef.IsDeleted = true;
        ef.DeletedAt = DateTime.UtcNow;
        ef.DeletedByUserId = deletedByUserId;
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<DomainSolidarity.EducationEnrollment>> GetEnrollmentsByProgramAsync(Guid programId, Guid organizationId)
    {
        var list = await EnrollmentQuery(organizationId)
            .Where(e => e.ProgramId == programId)
            .OrderBy(e => e.Associate.User.LastName).ThenBy(e => e.Associate.User.FirstName)
            .ToListAsync();
        return list.Select(MapEnrollment);
    }

    public async Task<IEnumerable<DomainSolidarity.EducationEnrollment>> GetEnrollmentsAsync(Guid organizationId)
    {
        var list = await EnrollmentQuery(organizationId).ToListAsync();
        return list.Select(MapEnrollment);
    }

    public async Task<DomainSolidarity.EducationEnrollment?> GetEnrollmentByIdAsync(Guid id, Guid organizationId)
    {
        var ef = await EnrollmentQuery(organizationId).FirstOrDefaultAsync(e => e.Id == id);
        return ef == null ? null : MapEnrollment(ef);
    }

    public Task<bool> EnrollmentExistsAsync(Guid programId, Guid userId, Guid organizationId) =>
        _context.EducationEnrollments.AnyAsync(e =>
            e.ProgramId == programId && e.UserId == userId && e.OrganizationId == organizationId && !e.IsDeleted);

    public async Task CreateEnrollmentAsync(DomainSolidarity.EducationEnrollment enrollment)
    {
        _context.EducationEnrollments.Add(new EfEntities.EducationEnrollment
        {
            Id = enrollment.Id,
            OrganizationId = enrollment.OrganizationId,
            ProgramId = enrollment.ProgramId,
            UserId = enrollment.UserId,
            Status = (byte)enrollment.Status,
            HoursCompleted = enrollment.HoursCompleted,
            CompletionDate = enrollment.CompletionDate,
            CreatedAt = enrollment.CreatedAt,
            CreatedByUserId = enrollment.CreatedByUserId
        });
        await _context.SaveChangesAsync();
    }

    public async Task UpdateEnrollmentAsync(DomainSolidarity.EducationEnrollment enrollment)
    {
        var ef = await _context.EducationEnrollments
            .FirstOrDefaultAsync(e => e.Id == enrollment.Id && e.OrganizationId == enrollment.OrganizationId && !e.IsDeleted);
        if (ef == null)
            return;

        ef.Status = (byte)enrollment.Status;
        ef.HoursCompleted = enrollment.HoursCompleted;
        ef.CompletionDate = enrollment.CompletionDate;
        ef.UpdatedAt = DateTime.UtcNow;
        ef.UpdatedByUserId = enrollment.UpdatedByUserId;
        await _context.SaveChangesAsync();
    }

    public async Task<decimal> GetCompletedBasicHoursAsync(Guid userId, Guid organizationId)
    {
        // Sum client-side to avoid provider differences on decimal aggregates
        var hours = await _context.EducationEnrollments
            .AsNoTracking()
            .Where(e => e.OrganizationId == organizationId && e.UserId == userId && !e.IsDeleted
                        && !e.Program.IsDeleted
                        && e.Status == (byte)EnrollmentStatus.Completed
                        && e.Program.Type == (byte)EducationProgramType.Basic)
            .Select(e => e.HoursCompleted)
            .ToListAsync();
        return hours.Sum();
    }

    private IQueryable<EfEntities.EducationEnrollment> EnrollmentQuery(Guid organizationId) =>
        _context.EducationEnrollments
            .AsNoTracking()
            .Include(e => e.Program)
            .Include(e => e.Associate).ThenInclude(a => a.User)
            .Where(e => e.OrganizationId == organizationId && !e.IsDeleted && !e.Program.IsDeleted);

    #region Mapping Helpers

    private static DomainSolidarity.EducationProgram MapProgram(EfEntities.EducationProgram ef) => new()
    {
        Id = ef.Id,
        OrganizationId = ef.OrganizationId,
        Name = ef.Name,
        Type = (EducationProgramType)ef.Type,
        RequiredHours = ef.RequiredHours,
        StartDate = ef.StartDate,
        EndDate = ef.EndDate,
        CreatedAt = ef.CreatedAt,
        CreatedByUserId = ef.CreatedByUserId,
        UpdatedAt = ef.UpdatedAt,
        UpdatedByUserId = ef.UpdatedByUserId
    };

    private static DomainSolidarity.EducationEnrollment MapEnrollment(EfEntities.EducationEnrollment ef) => new()
    {
        Id = ef.Id,
        OrganizationId = ef.OrganizationId,
        ProgramId = ef.ProgramId,
        UserId = ef.UserId,
        Status = (EnrollmentStatus)ef.Status,
        HoursCompleted = ef.HoursCompleted,
        CompletionDate = ef.CompletionDate,
        ProgramType = (EducationProgramType)ef.Program.Type,
        FullName = $"{ef.Associate.User.FirstName} {ef.Associate.User.LastName}".Trim(),
        CreatedAt = ef.CreatedAt,
        CreatedByUserId = ef.CreatedByUserId,
        UpdatedAt = ef.UpdatedAt,
        UpdatedByUserId = ef.UpdatedByUserId
    };

    #endregion
}
