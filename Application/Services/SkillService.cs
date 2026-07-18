namespace Application.Services;

using Application.Common.Exceptions;
using Application.DTOs.Skills;
using Application.Interfaces;
using Domain.Entities.Talent;
using Domain.Interfaces.Repositories;

/// <summary>
/// Servicio para gestión de catálogo de habilidades
/// </summary>
public class SkillService : ISkillService
{
    private readonly ISkillRepository _skillRepository;

    public SkillService(ISkillRepository skillRepository)
    {
        _skillRepository = skillRepository;
    }

    public async Task<IEnumerable<SkillDto>> GetAllSkillsAsync(Guid organizationId)
    {
        var skills = await _skillRepository.GetAllAsync(organizationId);

        return skills.Select(s => new SkillDto
        {
            Id = s.Id,
            Name = s.Name,
            Category = s.Category,
            SkillType = s.SkillType
        });
    }

    public async Task<Guid> CreateSkillAsync(SkillDto skillDto, Guid organizationId, Guid createdByUserId)
    {
        // Validar que no exista una skill con el mismo nombre (case insensitive)
        var exists = await _skillRepository.ExistsByNameAsync(skillDto.Name, organizationId);
        if (exists)
        {
            throw new BusinessValidationException($"Ya existe una habilidad con el nombre '{skillDto.Name}'");
        }

        var skill = new Skill
        {
            Name = skillDto.Name,
            Category = skillDto.Category,
            SkillType = skillDto.SkillType,
            OrganizationId = organizationId,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = createdByUserId
        };

        return await _skillRepository.CreateAsync(skill);
    }

    public async Task<bool> UpdateSkillAsync(SkillDto skillDto, Guid organizationId, Guid updatedByUserId)
    {
        var skill = await _skillRepository.GetByIdAsync(skillDto.Id);
        if (skill == null || (skill.OrganizationId != null && skill.OrganizationId != organizationId))
            return false;

        if (!string.Equals(skill.Name, skillDto.Name, StringComparison.OrdinalIgnoreCase))
        {
            var exists = await _skillRepository.ExistsByNameAsync(skillDto.Name, organizationId);
            if (exists)
                throw new BusinessValidationException($"Ya existe una habilidad con el nombre '{skillDto.Name}'");
        }

        skill.Name = skillDto.Name;
        skill.Category = skillDto.Category;
        skill.SkillType = skillDto.SkillType;
        skill.UpdatedAt = DateTime.UtcNow;
        skill.UpdatedByUserId = updatedByUserId;

        return await _skillRepository.UpdateAsync(skill);
    }

    public async Task<bool> DeleteSkillAsync(Guid skillId, Guid organizationId, Guid deletedByUserId)
    {
        return await _skillRepository.SoftDeleteAsync(skillId, organizationId, deletedByUserId);
    }

    public async Task<SkillDto?> GetSkillByIdAsync(Guid skillId, Guid organizationId)
    {
        var skill = await _skillRepository.GetByIdAsync(skillId);

        if (skill == null)
            return null;

        return new SkillDto
        {
            Id = skill.Id,
            Name = skill.Name,
            Category = skill.Category,
            SkillType = skill.SkillType
        };
    }

    public async Task<SkillDto?> GetSkillByNameAsync(string skillName, Guid organizationId)
    {
        var skills = await _skillRepository.GetAllAsync(organizationId);
        var skill = skills.FirstOrDefault(s => s.Name.Equals(skillName, StringComparison.OrdinalIgnoreCase));

        if (skill == null)
            return null;

        return new SkillDto
        {
            Id = skill.Id,
            Name = skill.Name,
            Category = skill.Category,
            SkillType = skill.SkillType
        };
    }
}
