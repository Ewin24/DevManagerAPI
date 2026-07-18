namespace Infrastructure.Repositories;

using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using DomainEntities = Domain.Entities.Talent;
using EfEntities = Infrastructure.Data.Entities;

/// <summary>
/// Repositorio para perfiles de empleados usando EF Core
/// </summary>
public class ProfileRepository : Domain.Interfaces.Repositories.IProfileRepository
{
    private readonly DevManagerDbContext _context;

    public ProfileRepository(DevManagerDbContext context)
    {
        _context = context;
    }

    public async Task<DomainEntities.EmployeeProfile?> GetByUserIdAsync(Guid userId, Guid organizationId)
    {
        var efProfile = await _context.EmployeeProfiles
            .AsNoTracking()
            .Where(p => p.UserId == userId
                        && p.OrganizationId == organizationId
                        && !p.IsDeleted)
            .FirstOrDefaultAsync();

        return efProfile != null ? MapToDomain(efProfile) : null;
    }

    public async Task<IEnumerable<DomainEntities.EmployeeProfile>> GetAllAsync(Guid organizationId)
    {
        // Cargar perfiles
        var efProfiles = await _context.EmployeeProfiles
            .AsNoTracking()
            .Where(p => p.OrganizationId == organizationId && !p.IsDeleted)
            .ToListAsync();

        if (!efProfiles.Any())
            return Enumerable.Empty<DomainEntities.EmployeeProfile>();

        var userIds = efProfiles.Select(p => p.UserId).ToList();

        // Cargar skills de esos usuarios
        var employeeSkills = await _context.EmployeeSkills
            .AsNoTracking()
            .Include(es => es.Skill)
            .Where(es => userIds.Contains(es.UserId)
                      && es.OrganizationId == organizationId
                      && !es.IsDeleted)
            .ToListAsync();

        // Mapear perfiles con sus skills
        return efProfiles.Select(profile =>
        {
            var profileSkills = employeeSkills.Where(es => es.UserId == profile.UserId).ToList();
            return MapToDomainWithSkills(profile, profileSkills);
        });
    }

    public async Task<bool> CreateAsync(DomainEntities.EmployeeProfile profile)
    {
        var existing = await _context.EmployeeProfiles
            .Where(p => p.UserId == profile.UserId
                        && p.OrganizationId == profile.OrganizationId)
            .FirstOrDefaultAsync();

        if (existing != null)
        {
            if (existing.IsDeleted)
            {
                existing.IsDeleted = false;
                existing.DeletedAt = null;
                existing.DeletedByUserId = null;
                existing.Bio = profile.Bio;
                existing.YearsExperience = profile.YearsExperience;
                existing.LinkedInUrl = profile.LinkedInUrl;
                existing.PortfolioUrl = profile.PortfolioUrl;
                existing.UpdatedAt = DateTime.UtcNow;
                existing.UpdatedByUserId = profile.UpdatedByUserId;
                return await _context.SaveChangesAsync() > 0;
            }
            return false;
        }

        var efProfile = new EfEntities.EmployeeProfile
        {
            UserId = profile.UserId,
            OrganizationId = profile.OrganizationId,
            Bio = profile.Bio,
            YearsExperience = profile.YearsExperience,
            LinkedInUrl = profile.LinkedInUrl,
            PortfolioUrl = profile.PortfolioUrl,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = profile.CreatedByUserId,
            UpdatedAt = DateTime.UtcNow,
            UpdatedByUserId = profile.UpdatedByUserId,
            IsDeleted = false
        };
        await _context.EmployeeProfiles.AddAsync(efProfile);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpsertAsync(DomainEntities.EmployeeProfile profile)
    {
        var existing = await _context.EmployeeProfiles
            .Where(p => p.UserId == profile.UserId
                        && p.OrganizationId == profile.OrganizationId)
            .FirstOrDefaultAsync();

        if (existing != null)
        {
            if (existing.IsDeleted)
            {
                existing.IsDeleted = false;
                existing.DeletedAt = null;
                existing.DeletedByUserId = null;
            }
            existing.Bio = profile.Bio;
            existing.YearsExperience = profile.YearsExperience;
            existing.LinkedInUrl = profile.LinkedInUrl;
            existing.PortfolioUrl = profile.PortfolioUrl;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.UpdatedByUserId = profile.UpdatedByUserId;
        }
        else
        {
            // Insert
            var efProfile = new EfEntities.EmployeeProfile
            {
                UserId = profile.UserId,
                OrganizationId = profile.OrganizationId,
                Bio = profile.Bio,
                YearsExperience = profile.YearsExperience,
                LinkedInUrl = profile.LinkedInUrl,
                PortfolioUrl = profile.PortfolioUrl,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = profile.CreatedByUserId,
                UpdatedAt = DateTime.UtcNow,
                UpdatedByUserId = profile.UpdatedByUserId,
                IsDeleted = false
            };
            await _context.EmployeeProfiles.AddAsync(efProfile);
        }

        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> SoftDeleteAsync(Guid userId, Guid organizationId, Guid deletedByUserId)
    {
        var efProfile = await _context.EmployeeProfiles
            .FirstOrDefaultAsync(p =>
                p.UserId == userId &&
                p.OrganizationId == organizationId &&
                !p.IsDeleted);

        if (efProfile == null)
            return false;

        efProfile.IsDeleted = true;
        efProfile.DeletedAt = DateTime.UtcNow;
        efProfile.DeletedByUserId = deletedByUserId;

        return await _context.SaveChangesAsync() > 0;
    }

    private static DomainEntities.EmployeeProfile MapToDomain(EfEntities.EmployeeProfile ef)
    {
        return new DomainEntities.EmployeeProfile
        {
            UserId = ef.UserId,
            OrganizationId = ef.OrganizationId,
            Bio = ef.Bio,
            YearsExperience = ef.YearsExperience,
            LinkedInUrl = ef.LinkedInUrl,
            PortfolioUrl = ef.PortfolioUrl,
            CreatedAt = ef.CreatedAt,
            CreatedByUserId = ef.CreatedByUserId,
            UpdatedAt = ef.UpdatedAt,
            UpdatedByUserId = ef.UpdatedByUserId,
            IsDeleted = ef.IsDeleted,
            DeletedAt = ef.DeletedAt,
            DeletedByUserId = ef.DeletedByUserId
        };
    }
    private static DomainEntities.EmployeeProfile MapToDomainWithSkills(
        EfEntities.EmployeeProfile ef,
        List<EfEntities.EmployeeSkill> employeeSkills)
    {
        var profile = MapToDomain(ef);

        // Agregar información de skills desde la lista proporcionada
        if (employeeSkills?.Any() == true)
        {
            profile.EmployeeSkills = employeeSkills.Select(es => new DomainEntities.EmployeeSkill
            {
                Id = es.Id,
                OrganizationId = es.OrganizationId,
                UserId = es.UserId,
                SkillId = es.SkillId,
                Level = es.Level,
                EvidenceUrl = es.EvidenceUrl,
                LastValidatedAt = es.LastValidatedAt,
                Skill = es.Skill != null ? new DomainEntities.Skill
                {
                    Id = es.Skill.Id,
                    Name = es.Skill.Name,
                    Category = es.Skill.Category,
                    SkillType = es.Skill.SkillType,
                    OrganizationId = es.Skill.OrganizationId
                } : null
            }).ToList();
        }

        return profile;
    }
}