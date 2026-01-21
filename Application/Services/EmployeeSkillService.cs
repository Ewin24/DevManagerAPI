namespace Application.Services;

using Application.DTOs.Skills;
using Application.Interfaces;
using Domain.Entities.Talent;
using Domain.Interfaces.Repositories;

/// <summary>
/// Servicio para gestión de habilidades de empleados
/// </summary>
public class EmployeeSkillService : IEmployeeSkillService
{
    private readonly IEmployeeSkillRepository _employeeSkillRepository;
    private readonly ISkillRepository _skillRepository;

    public EmployeeSkillService(
        IEmployeeSkillRepository employeeSkillRepository,
        ISkillRepository skillRepository)
    {
        _employeeSkillRepository = employeeSkillRepository;
        _skillRepository = skillRepository;
    }

    public async Task<IEnumerable<EmployeeSkillResponse>> GetEmployeeSkillsAsync(Guid userId, Guid organizationId)
    {
        var skills = await _employeeSkillRepository.GetByUserIdAsync(userId, organizationId);

        return skills.Select(es => new EmployeeSkillResponse
        {
            Id = es.Id,
            SkillId = es.SkillId,
            SkillName = es.SkillName,
            Category = es.SkillCategory,
            Level = es.Level,
            EvidenceUrl = es.EvidenceUrl,
            LastValidatedAt = es.LastValidatedAt,
            ValidatedByUserId = es.ValidatedByUserId
        });
    }

    public async Task<bool> UpsertEmployeeSkillAsync(Guid userId, Guid organizationId, UpsertEmployeeSkillRequest request)
    {
        // Verificar que la skill existe
        var skill = await _skillRepository.GetByIdAsync(request.SkillId);
        if (skill == null)
        {
            throw new Common.Exceptions.NotFoundException($"Skill con ID {request.SkillId} no existe");
        }

        var employeeSkill = new EmployeeSkill
        {
            UserId = userId,
            SkillId = request.SkillId,
            OrganizationId = organizationId,
            Level = request.Level,
            EvidenceUrl = request.EvidenceUrl
        };

        var id = await _employeeSkillRepository.UpsertAsync(employeeSkill);
        return id != Guid.Empty;
    }

    public async Task<bool> ValidateSkillAsync(Guid employeeSkillId, Guid organizationId, Guid? validatorUserId, byte? newLevel = null)
    {
        return await _employeeSkillRepository.ValidateSkillAsync(employeeSkillId, organizationId, validatorUserId, newLevel);
    }

    public async Task<IEnumerable<EmployeeSkillResponse>> GetSkillsByUserIdAsync(Guid userId, Guid organizationId)
    {
        return await GetEmployeeSkillsAsync(userId, organizationId);
    }
}
