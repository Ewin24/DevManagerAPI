namespace Application.Interfaces;

using Application.DTOs.Profiles;
using Application.DTOs.Skills;

/// <summary>
/// Servicio para gestión de perfiles de empleados
/// </summary>
public interface IProfileService
{
    Task<EmployeeProfileDto?> GetMyProfileAsync(Guid userId, Guid organizationId);
    Task<bool> UpdateMyProfileAsync(Guid userId, Guid organizationId, UpdateProfileRequest request);

    // Métodos para el agente
    Task<EmployeeProfileDto?> GetProfileByIdAsync(Guid userId, Guid organizationId);
    Task<IEnumerable<ProfileWithSkillsDto>> GetAllProfilesWithSkillsAsync(Guid organizationId);
}

/// <summary>
/// Servicio para gestión de catálogo de habilidades
/// </summary>
public interface ISkillService
{
    Task<IEnumerable<SkillDto>> GetAllSkillsAsync(Guid organizationId);
    Task<Guid> CreateSkillAsync(SkillDto skillDto, Guid organizationId);

    // Métodos para el agente
    Task<SkillDto?> GetSkillByIdAsync(Guid skillId, Guid organizationId);
    Task<SkillDto?> GetSkillByNameAsync(string skillName, Guid organizationId);
}

/// <summary>
/// Servicio para gestión de habilidades de empleados
/// </summary>
public interface IEmployeeSkillService
{
    Task<IEnumerable<EmployeeSkillResponse>> GetEmployeeSkillsAsync(Guid userId, Guid organizationId);
    Task<bool> UpsertEmployeeSkillAsync(Guid userId, Guid organizationId, UpsertEmployeeSkillRequest request);
    Task<bool> ValidateSkillAsync(Guid employeeSkillId, Guid organizationId, Guid? validatorUserId, byte? newLevel = null);

    // Métodos para el agente
    Task<IEnumerable<EmployeeSkillResponse>> GetSkillsByUserIdAsync(Guid userId, Guid organizationId);
}
