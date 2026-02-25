namespace Application.Services;

using Application.DTOs.Profiles;
using Application.Interfaces;
using Domain.Entities.Talent;
using Domain.Interfaces.Repositories;

/// <summary>
/// Servicio para gestión de perfiles de empleados
/// </summary>
public class ProfileService : IProfileService
{
    private readonly IProfileRepository _profileRepository;
    private readonly IEmployeeSkillRepository _employeeSkillRepository;
    private readonly ISkillRepository _skillRepository;
    private readonly ICertificationRepository _certificationRepository;

    public ProfileService(
        IProfileRepository profileRepository,
        IEmployeeSkillRepository employeeSkillRepository,
        ISkillRepository skillRepository,
        ICertificationRepository certificationRepository)
    {
        _profileRepository = profileRepository;
        _employeeSkillRepository = employeeSkillRepository;
        _skillRepository = skillRepository;
        _certificationRepository = certificationRepository;
    }

    public async Task<EmployeeProfileDto?> GetMyProfileAsync(Guid userId, Guid organizationId)
    {
        var profile = await _profileRepository.GetByUserIdAsync(userId, organizationId);

        if (profile == null)
            return null;

        // cargar certificaciones
        var certs = await _certificationRepository.GetCertificationsByUserIdAsync(userId, organizationId);

        return new EmployeeProfileDto
        {
            UserId = profile.UserId,
            Bio = profile.Bio,
            YearsExperience = profile.YearsExperience,
            LinkedInUrl = profile.LinkedInUrl,
            PortfolioUrl = profile.PortfolioUrl,
            CreatedAt = profile.CreatedAt,
            UpdatedAt = profile.UpdatedAt,
            Certifications = certs.Select(c => new CertificationDto
            {
                Id = c.Id,
                UserId = c.UserId,
                Name = c.Name,
                Issuer = c.Issuer,
                IssueDate = c.IssueDate,
                ExpirationDate = c.ExpirationDate,
                EvidenceUrl = c.EvidenceUrl
            })
        };
    }

    public async Task<bool> UpdateMyProfileAsync(Guid userId, Guid organizationId, UpdateProfileRequest request)
    {
        var profile = new EmployeeProfile
        {
            UserId = userId,
            OrganizationId = organizationId,
            Bio = request.Bio,
            YearsExperience = request.YearsExperience,
            LinkedInUrl = request.LinkedInUrl,
            PortfolioUrl = request.PortfolioUrl,
            UpdatedAt = DateTime.UtcNow,
            UpdatedByUserId = userId
        };

        return await _profileRepository.UpsertAsync(profile);
    }

    public async Task<bool> DeleteMyProfileAsync(Guid userId, Guid organizationId, Guid deletedByUserId)
    {
        return await _profileRepository.SoftDeleteAsync(userId, organizationId, deletedByUserId);
    }

    public async Task<EmployeeProfileDto?> GetProfileByIdAsync(Guid userId, Guid organizationId)
    {
        return await GetMyProfileAsync(userId, organizationId);
    }

    public async Task<IEnumerable<ProfileWithSkillsDto>> GetAllProfilesWithSkillsAsync(Guid organizationId)
    {
        var profiles = await _profileRepository.GetAllAsync(organizationId);

        // for each profile, retrieve certifications separately
        var result = new List<ProfileWithSkillsDto>();
        foreach (var p in profiles)
        {
            var certs = await _certificationRepository.GetCertificationsByUserIdAsync(p.UserId, organizationId);
            result.Add(new ProfileWithSkillsDto
            {
                UserId = p.UserId,
                Bio = p.Bio,
                YearsExperience = p.YearsExperience.HasValue ? (byte?)p.YearsExperience.Value : null,
                Skills = p.EmployeeSkills?.Select(es => new EmployeeSkillSummary
                {
                    SkillId = es.SkillId,
                    SkillName = es.Skill?.Name ?? "Unknown",
                    CurrentLevel = es.Level,
                    LastValidatedAt = es.LastValidatedAt
                }) ?? Enumerable.Empty<EmployeeSkillSummary>(),
                Certifications = certs.Select(c => new CertificationDto
                {
                    Id = c.Id,
                    UserId = c.UserId,
                    Name = c.Name,
                    Issuer = c.Issuer,
                    IssueDate = c.IssueDate,
                    ExpirationDate = c.ExpirationDate,
                    EvidenceUrl = c.EvidenceUrl
                })
            });
        }
        return result;
    }
}