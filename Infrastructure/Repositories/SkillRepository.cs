namespace Infrastructure.Repositories;

using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using DomainEntities = Domain.Entities.Talent;
using EfEntities = Infrastructure.Data.Entities;

/// <summary>
/// Repositorio para catálogo de habilidades usando EF Core
/// </summary>
public class SkillRepository : Domain.Interfaces.Repositories.ISkillRepository
{
    private readonly DevManagerDbContext _context;

    public SkillRepository(DevManagerDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<DomainEntities.Skill>> GetAllAsync(Guid organizationId)
    {
        var efSkills = await _context.Skills
            .AsNoTracking()
            .Where(s => (s.OrganizationId == null || s.OrganizationId == organizationId)
                        && !s.IsDeleted)
            .OrderBy(s => s.Name)
            .ToListAsync();

        return efSkills.Select(MapToDomain);
    }

    public async Task<DomainEntities.Skill?> GetByIdAsync(Guid id)
    {
        var efSkill = await _context.Skills
            .AsNoTracking()
            .Where(s => s.Id == id && !s.IsDeleted)
            .FirstOrDefaultAsync();

        return efSkill != null ? MapToDomain(efSkill) : null;
    }

    public async Task<Guid> CreateAsync(DomainEntities.Skill skill)
    {
        var efSkill = new EfEntities.Skill
        {
            Id = Guid.NewGuid(),
            Name = skill.Name,
            Category = skill.Category,
            SkillType = skill.SkillType,
            OrganizationId = skill.OrganizationId,
            CreatedAt = skill.CreatedAt == default ? DateTime.UtcNow : skill.CreatedAt,
            CreatedByUserId = skill.CreatedByUserId,
            IsDeleted = false
        };

        await _context.Skills.AddAsync(efSkill);
        await _context.SaveChangesAsync();

        return efSkill.Id;
    }

    public async Task<bool> ExistsByNameAsync(string name, Guid? organizationId)
    {
        return await _context.Skills
            .Where(s => s.Name.ToLower() == name.ToLower()
                        && s.OrganizationId == organizationId
                        && !s.IsDeleted)
            .AnyAsync();
    }

    public async Task<bool> UpdateAsync(DomainEntities.Skill skill)
    {
        var efSkill = await _context.Skills
            .FirstOrDefaultAsync(s => s.Id == skill.Id && !s.IsDeleted);

        if (efSkill == null)
            return false;

        efSkill.Name = skill.Name;
        efSkill.Category = skill.Category;
        efSkill.SkillType = skill.SkillType;
        efSkill.UpdatedAt = DateTime.UtcNow;
        efSkill.UpdatedByUserId = skill.UpdatedByUserId;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SoftDeleteAsync(Guid id, Guid organizationId, Guid deletedByUserId)
    {
        var efSkill = await _context.Skills
            .FirstOrDefaultAsync(s => s.Id == id && s.OrganizationId == organizationId && !s.IsDeleted);

        if (efSkill == null)
            return false;

        efSkill.IsDeleted = true;
        efSkill.DeletedAt = DateTime.UtcNow;
        efSkill.DeletedByUserId = deletedByUserId;

        await _context.SaveChangesAsync();
        return true;
    }

    private static DomainEntities.Skill MapToDomain(EfEntities.Skill ef)
    {
        return new DomainEntities.Skill
        {
            Id = ef.Id,
            Name = ef.Name,
            Category = ef.Category,
            SkillType = ef.SkillType,
            OrganizationId = ef.OrganizationId,
            CreatedAt = ef.CreatedAt,
            CreatedByUserId = ef.CreatedByUserId,
            UpdatedAt = ef.UpdatedAt,
            UpdatedByUserId = ef.UpdatedByUserId,
            IsDeleted = ef.IsDeleted,
            DeletedAt = ef.DeletedAt,
            DeletedByUserId = ef.DeletedByUserId
        };
    }
}