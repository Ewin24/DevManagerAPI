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

    public ProfileService(IProfileRepository profileRepository)
    {
        _profileRepository = profileRepository;
    }

    public async Task<EmployeeProfileDto?> GetMyProfileAsync(Guid userId, Guid organizationId)
    {
        var profile = await _profileRepository.GetByUserIdAsync(userId, organizationId);

        if (profile == null)
            return null;

        return new EmployeeProfileDto
        {
            UserId = profile.UserId,
            Bio = profile.Bio,
            YearsExperience = profile.YearsExperience,
            LinkedInUrl = profile.LinkedInUrl,
            PortfolioUrl = profile.PortfolioUrl
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
            PortfolioUrl = request.PortfolioUrl
        };

        return await _profileRepository.UpsertAsync(profile);
    }
}
