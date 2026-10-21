namespace Infrastructure.Repositories;

using Domain.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using DomainSolidarity = Domain.Entities.Solidarity;
using EfEntities = Infrastructure.Data.Entities;

/// <summary>
/// Repositorio de politicas de datos y consentimientos usando EF Core (siempre filtrado por OrganizationId)
/// </summary>
public class ConsentRepository : IConsentRepository
{
    private readonly DevManagerDbContext _context;

    public ConsentRepository(DevManagerDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<DomainSolidarity.DataPolicyVersion>> GetPoliciesAsync(Guid organizationId)
    {
        var efPolicies = await _context.DataPolicyVersions
            .AsNoTracking()
            .Where(p => p.OrganizationId == organizationId && !p.IsDeleted)
            .OrderByDescending(p => p.EffectiveFrom).ThenByDescending(p => p.CreatedAt)
            .ToListAsync();

        return efPolicies.Select(MapPolicy);
    }

    public async Task<DomainSolidarity.DataPolicyVersion?> GetPolicyByIdAsync(Guid id, Guid organizationId)
    {
        var ef = await _context.DataPolicyVersions
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id && p.OrganizationId == organizationId && !p.IsDeleted);

        return ef == null ? null : MapPolicy(ef);
    }

    public async Task<DomainSolidarity.DataPolicyVersion?> GetCurrentPolicyAsync(Guid organizationId)
    {
        var ef = await _context.DataPolicyVersions
            .AsNoTracking()
            .Where(p => p.OrganizationId == organizationId && !p.IsDeleted)
            .OrderByDescending(p => p.EffectiveFrom).ThenByDescending(p => p.CreatedAt)
            .FirstOrDefaultAsync();

        return ef == null ? null : MapPolicy(ef);
    }

    public async Task<bool> PolicyVersionExistsAsync(Guid organizationId, string version)
    {
        return await _context.DataPolicyVersions
            .AnyAsync(p => p.OrganizationId == organizationId && p.Version == version && !p.IsDeleted);
    }

    public async Task CreatePolicyAsync(DomainSolidarity.DataPolicyVersion policy)
    {
        _context.DataPolicyVersions.Add(new EfEntities.DataPolicyVersion
        {
            Id = policy.Id,
            OrganizationId = policy.OrganizationId,
            Version = policy.Version,
            Purpose = policy.Purpose,
            Text = policy.Text,
            EffectiveFrom = policy.EffectiveFrom,
            CreatedAt = policy.CreatedAt,
            CreatedByUserId = policy.CreatedByUserId
        });
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<DomainSolidarity.DataConsent>> GetConsentsAsync(Guid userId, Guid organizationId)
    {
        var efConsents = await _context.DataConsents
            .AsNoTracking()
            .Include(c => c.PolicyVersion)
            .Where(c => c.UserId == userId && c.OrganizationId == organizationId)
            .OrderByDescending(c => c.GrantedAt)
            .ToListAsync();

        return efConsents.Select(MapConsent);
    }

    public async Task<DomainSolidarity.DataConsent?> GetConsentByIdAsync(Guid id, Guid organizationId)
    {
        var ef = await _context.DataConsents
            .AsNoTracking()
            .Include(c => c.PolicyVersion)
            .FirstOrDefaultAsync(c => c.Id == id && c.OrganizationId == organizationId);

        return ef == null ? null : MapConsent(ef);
    }

    public async Task<bool> HasActiveConsentAsync(Guid userId, Guid organizationId)
    {
        return await _context.DataConsents
            .AnyAsync(c => c.UserId == userId && c.OrganizationId == organizationId && c.RevokedAt == null);
    }

    public async Task<bool> HasActiveConsentForPolicyAsync(Guid userId, Guid policyVersionId, Guid organizationId)
    {
        return await _context.DataConsents
            .AnyAsync(c => c.UserId == userId && c.PolicyVersionId == policyVersionId &&
                           c.OrganizationId == organizationId && c.RevokedAt == null);
    }

    public async Task CreateConsentAsync(DomainSolidarity.DataConsent consent)
    {
        _context.DataConsents.Add(new EfEntities.DataConsent
        {
            Id = consent.Id,
            OrganizationId = consent.OrganizationId,
            UserId = consent.UserId,
            PolicyVersionId = consent.PolicyVersionId,
            GrantedAt = consent.GrantedAt,
            GrantedByUserId = consent.GrantedByUserId,
            CreatedAt = consent.GrantedAt,
            CreatedByUserId = consent.GrantedByUserId
        });
        await _context.SaveChangesAsync();
    }

    public async Task UpdateConsentAsync(DomainSolidarity.DataConsent consent)
    {
        var ef = await _context.DataConsents
            .FirstOrDefaultAsync(c => c.Id == consent.Id && c.OrganizationId == consent.OrganizationId);

        if (ef == null)
            return;

        // Append-only: solo cambian los campos de revocacion
        ef.RevokedAt = consent.RevokedAt;
        ef.RevokedByUserId = consent.RevokedByUserId;
        ef.RevocationReason = consent.RevocationReason;
        await _context.SaveChangesAsync();
    }

    #region Mapping Helpers

    private static DomainSolidarity.DataPolicyVersion MapPolicy(EfEntities.DataPolicyVersion ef) => new()
    {
        Id = ef.Id,
        OrganizationId = ef.OrganizationId,
        Version = ef.Version,
        Purpose = ef.Purpose,
        Text = ef.Text,
        EffectiveFrom = ef.EffectiveFrom,
        CreatedAt = ef.CreatedAt,
        CreatedByUserId = ef.CreatedByUserId
    };

    private static DomainSolidarity.DataConsent MapConsent(EfEntities.DataConsent ef) => new()
    {
        Id = ef.Id,
        OrganizationId = ef.OrganizationId,
        UserId = ef.UserId,
        PolicyVersionId = ef.PolicyVersionId,
        GrantedAt = ef.GrantedAt,
        GrantedByUserId = ef.GrantedByUserId,
        RevokedAt = ef.RevokedAt,
        RevokedByUserId = ef.RevokedByUserId,
        RevocationReason = ef.RevocationReason,
        PolicyVersion = ef.PolicyVersion == null ? null : MapPolicy(ef.PolicyVersion)
    };

    #endregion
}
