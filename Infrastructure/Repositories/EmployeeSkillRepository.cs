namespace Infrastructure.Repositories;

using Infrastructure.Data;
using EfEntities = Infrastructure.Data.Entities;
using DomainEntities = Domain.Entities.Talent;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repositorio para habilidades de empleados usando EF Core
/// </summary>
public class EmployeeSkillRepository : Domain.Interfaces.Repositories.IEmployeeSkillRepository
{
    private readonly DevManagerDbContext _context;

    public EmployeeSkillRepository(DevManagerDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<DomainEntities.EmployeeSkill>> GetByUserIdAsync(Guid userId, Guid organizationId)
    {
        var efSkills = await _context.EmployeeSkills
            .AsNoTracking()
            .Include(es => es.Skill)
            .Where(es => es.UserId == userId 
                         && es.OrganizationId == organizationId 
                         && !es.IsDeleted)
            .ToListAsync();

        return efSkills.Select(MapToDomain);
    }

    public async Task<DomainEntities.EmployeeSkill?> GetByIdAsync(Guid id)
    {
        var efSkill = await _context.EmployeeSkills
            .AsNoTracking()
            .Include(es => es.Skill)
            .Where(es => es.Id == id && !es.IsDeleted)
            .FirstOrDefaultAsync();

        return efSkill != null ? MapToDomain(efSkill) : null;
    }

    public async Task<Guid> UpsertAsync(DomainEntities.EmployeeSkill employeeSkill)
    {
        var existing = await _context.EmployeeSkills
            .Where(es => es.UserId == employeeSkill.UserId 
                         && es.SkillId == employeeSkill.SkillId
                         && es.OrganizationId == employeeSkill.OrganizationId
                         && !es.IsDeleted)
            .FirstOrDefaultAsync();

        if (existing != null)
        {
            // Update existing
            existing.Level = employeeSkill.Level;
            existing.EvidenceUrl = employeeSkill.EvidenceUrl;
            existing.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return existing.Id;
        }
        else
        {
            // Create new
            var efSkill = new EfEntities.EmployeeSkill
            {
                Id = Guid.NewGuid(),
                UserId = employeeSkill.UserId,
                SkillId = employeeSkill.SkillId,
                OrganizationId = employeeSkill.OrganizationId,
                Level = employeeSkill.Level,
                EvidenceUrl = employeeSkill.EvidenceUrl,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            };
            await _context.EmployeeSkills.AddAsync(efSkill);
            await _context.SaveChangesAsync();
            return efSkill.Id;
        }
    }

    public async Task<bool> ValidateSkillAsync(Guid id, Guid organizationId, Guid? validatorUserId, byte? newLevel = null)
    {
        var skill = await _context.EmployeeSkills
            .Where(es => es.Id == id && es.OrganizationId == organizationId && !es.IsDeleted)
            .FirstOrDefaultAsync();

        if (skill == null)
            return false;

        skill.LastValidatedAt = DateTime.UtcNow;
        skill.ValidatedByUserId = validatorUserId;
        if (newLevel.HasValue)
        {
            skill.Level = newLevel.Value;
        }
        skill.UpdatedAt = DateTime.UtcNow;

        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateLevelAsync(Guid userId, Guid skillId, byte newLevel, Guid organizationId)
    {
        var skill = await _context.EmployeeSkills
            .Where(es => es.UserId == userId 
                         && es.SkillId == skillId 
                         && es.OrganizationId == organizationId 
                         && !es.IsDeleted)
            .FirstOrDefaultAsync();

        if (skill == null)
            return false;

        skill.Level = newLevel;
        skill.UpdatedAt = DateTime.UtcNow;

        return await _context.SaveChangesAsync() > 0;
    }

    private static DomainEntities.EmployeeSkill MapToDomain(EfEntities.EmployeeSkill ef)
    {
        return new DomainEntities.EmployeeSkill
        {
            Id = ef.Id,
            UserId = ef.UserId,
            SkillId = ef.SkillId,
            OrganizationId = ef.OrganizationId,
            SkillName = ef.Skill?.Name ?? string.Empty,
            SkillCategory = ef.Skill?.Category,
            Level = ef.Level,
            EvidenceUrl = ef.EvidenceUrl,
            LastValidatedAt = ef.LastValidatedAt,
            ValidatedByUserId = ef.ValidatedByUserId
        };
    }
}
