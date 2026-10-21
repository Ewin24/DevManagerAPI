namespace Application.Services;

using Application.Common.Exceptions;
using Application.DTOs.Solidarity;
using Application.Interfaces;
using Domain.Entities.Solidarity;
using Domain.Interfaces.Repositories;

/// <summary>
/// Servicio de políticas de datos y consentimientos (append-only, auditado)
/// </summary>
public class ConsentService : IConsentService
{
    private readonly IConsentRepository _consentRepository;
    private readonly IUserRepository _userRepository;

    public ConsentService(IConsentRepository consentRepository, IUserRepository userRepository)
    {
        _consentRepository = consentRepository;
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<DataPolicyDto>> GetPoliciesAsync(Guid organizationId)
    {
        var policies = await _consentRepository.GetPoliciesAsync(organizationId);
        return policies.Select(MapPolicy).ToList();
    }

    public async Task<DataPolicyDto?> GetCurrentPolicyAsync(Guid organizationId)
    {
        var policy = await _consentRepository.GetCurrentPolicyAsync(organizationId);
        return policy == null ? null : MapPolicy(policy);
    }

    public async Task<DataPolicyDto> CreatePolicyAsync(CreateDataPolicyRequest request, Guid organizationId, Guid createdByUserId)
    {
        var version = request.Version.Trim();
        if (version.Length == 0 || string.IsNullOrWhiteSpace(request.Purpose) || string.IsNullOrWhiteSpace(request.Text))
            throw new BusinessValidationException("Versión, finalidad y texto son obligatorios");

        if (await _consentRepository.PolicyVersionExistsAsync(organizationId, version))
            throw new ConflictException($"La versión de política '{version}' ya existe");

        var policy = new DataPolicyVersion
        {
            Id = Guid.NewGuid(),
            OrganizationId = organizationId,
            Version = version,
            Purpose = request.Purpose.Trim(),
            Text = request.Text,
            EffectiveFrom = request.EffectiveFrom ?? DateOnly.FromDateTime(DateTime.UtcNow),
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = createdByUserId
        };

        await _consentRepository.CreatePolicyAsync(policy);
        return MapPolicy(policy);
    }

    public async Task UpdatePolicyAsync(Guid policyId, Guid organizationId)
    {
        _ = await _consentRepository.GetPolicyByIdAsync(policyId, organizationId)
            ?? throw new NotFoundException("Política de datos", policyId);

        throw new ConflictException("Una política publicada es inmutable; publique una nueva versión");
    }

    public async Task<IEnumerable<ConsentDto>> GetUserConsentsAsync(Guid userId, Guid organizationId)
    {
        await EnsureUserAsync(userId, organizationId);
        var consents = await _consentRepository.GetConsentsAsync(userId, organizationId);
        return consents.Select(MapConsent).ToList();
    }

    public async Task<ConsentDto> GrantAsync(Guid userId, GrantConsentRequest request, Guid organizationId, Guid grantedByUserId)
    {
        await EnsureUserAsync(userId, organizationId);
        var policy = await _consentRepository.GetPolicyByIdAsync(request.PolicyVersionId, organizationId)
            ?? throw new NotFoundException("Política de datos", request.PolicyVersionId);

        if (await _consentRepository.HasActiveConsentForPolicyAsync(userId, policy.Id, organizationId))
            throw new ConflictException("El usuario ya tiene un consentimiento activo para esta versión de la política");

        var consent = new DataConsent
        {
            Id = Guid.NewGuid(),
            OrganizationId = organizationId,
            UserId = userId,
            PolicyVersionId = policy.Id,
            GrantedAt = DateTime.UtcNow,
            GrantedByUserId = grantedByUserId,
            PolicyVersion = policy
        };

        await _consentRepository.CreateConsentAsync(consent);
        return MapConsent(consent);
    }

    public async Task<ConsentDto> RevokeAsync(Guid consentId, RevokeConsentRequest request, Guid organizationId, Guid revokedByUserId)
    {
        var consent = await _consentRepository.GetConsentByIdAsync(consentId, organizationId)
            ?? throw new NotFoundException("Consentimiento", consentId);

        var reason = request.Reason?.Trim();
        if (string.IsNullOrEmpty(reason))
            throw new BusinessValidationException("El motivo de revocación es obligatorio");
        if (reason.Length > 400)
            throw new BusinessValidationException("El motivo no puede superar 400 caracteres");
        if (!consent.IsActive)
            throw new ConflictException("El consentimiento ya fue revocado");

        consent.RevokedAt = DateTime.UtcNow;
        consent.RevokedByUserId = revokedByUserId;
        consent.RevocationReason = reason;

        await _consentRepository.UpdateConsentAsync(consent);
        return MapConsent(consent);
    }

    #region Helpers

    private async Task EnsureUserAsync(Guid userId, Guid organizationId)
    {
        if (await _userRepository.GetByIdAsync(userId, organizationId) == null)
            throw new NotFoundException("Usuario", userId);
    }

    private static DataPolicyDto MapPolicy(DataPolicyVersion p) => new()
    {
        Id = p.Id,
        Version = p.Version,
        Purpose = p.Purpose,
        Text = p.Text,
        EffectiveFrom = p.EffectiveFrom,
        CreatedAt = p.CreatedAt
    };

    private static ConsentDto MapConsent(DataConsent c) => new()
    {
        Id = c.Id,
        UserId = c.UserId,
        PolicyVersionId = c.PolicyVersionId,
        PolicyVersion = c.PolicyVersion?.Version ?? string.Empty,
        Purpose = c.PolicyVersion?.Purpose ?? string.Empty,
        GrantedAt = c.GrantedAt,
        GrantedByUserId = c.GrantedByUserId,
        RevokedAt = c.RevokedAt,
        RevokedByUserId = c.RevokedByUserId,
        RevocationReason = c.RevocationReason,
        IsActive = c.IsActive
    };

    #endregion
}
