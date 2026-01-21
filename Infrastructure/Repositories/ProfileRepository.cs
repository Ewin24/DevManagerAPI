namespace Infrastructure.Repositories;

using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using EfEntities = Infrastructure.Data.Entities;
using DomainEntities = Domain.Entities.Talent;

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

    public async Task<bool> UpsertAsync(DomainEntities.EmployeeProfile profile)
    {
        var existing = await _context.EmployeeProfiles
            .Where(p => p.UserId == profile.UserId 
                        && p.OrganizationId == profile.OrganizationId)
            .FirstOrDefaultAsync();

        if (existing != null)
        {
            // Update
            existing.Bio = profile.Bio;
            existing.YearsExperience = profile.YearsExperience;
            existing.LinkedInUrl = profile.LinkedInUrl;
            existing.PortfolioUrl = profile.PortfolioUrl;
            existing.UpdatedAt = DateTime.UtcNow;
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
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            };
            await _context.EmployeeProfiles.AddAsync(efProfile);
        }

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
            PortfolioUrl = ef.PortfolioUrl
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