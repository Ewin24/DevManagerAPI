namespace Infrastructure.Repositories;

using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using DomainEntities = Domain.Entities.Talent;
using EfEntities = Infrastructure.Data.Entities;

/// <summary>
/// Repositorio para certificaciones usando EF Core
/// </summary>
public class CertificationRepository : Domain.Interfaces.Repositories.ICertificationRepository
{
    private readonly DevManagerDbContext _context;

    public CertificationRepository(DevManagerDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<DomainEntities.Certification>> GetCertificationsByUserIdAsync(Guid userId, Guid organizationId)
    {
        var efCerts = await _context.Certifications
            .AsNoTracking()
            .Where(c => c.UserId == userId
                        && c.OrganizationId == organizationId
                        && !c.IsDeleted)
            .ToListAsync();

        return efCerts.Select(MapToDomain);
    }

    public async Task<DomainEntities.Certification?> GetCertificationByIdAsync(Guid id)
    {
        var ef = await _context.Certifications
            .AsNoTracking()
            .Where(c => c.Id == id && !c.IsDeleted)
            .FirstOrDefaultAsync();

        return ef != null ? MapToDomain(ef) : null;
    }

    public async Task<Guid> CreateCertificationAsync(DomainEntities.Certification certification)
    {
        var ef = new EfEntities.Certification
        {
            Id = Guid.NewGuid(),
            UserId = certification.UserId,
            OrganizationId = certification.OrganizationId,
            Name = certification.Name,
            Issuer = certification.Issuer,
            IssueDate = certification.IssueDate,
            ExpirationDate = certification.ExpirationDate,
            EvidenceUrl = certification.EvidenceUrl,
            CreatedAt = certification.CreatedAt == default ? DateTime.UtcNow : certification.CreatedAt,
            CreatedByUserId = certification.CreatedByUserId,
            IsDeleted = false
        };

        await _context.Certifications.AddAsync(ef);
        await _context.SaveChangesAsync();
        return ef.Id;
    }

    public async Task<bool> UpdateCertificationAsync(DomainEntities.Certification certification)
    {
        var ef = await _context.Certifications
            .FirstOrDefaultAsync(c => c.Id == certification.Id && !c.IsDeleted);

        if (ef == null) return false;

        ef.Name = certification.Name;
        ef.Issuer = certification.Issuer;
        ef.IssueDate = certification.IssueDate;
        ef.ExpirationDate = certification.ExpirationDate;
        ef.EvidenceUrl = certification.EvidenceUrl;
        ef.UpdatedAt = DateTime.UtcNow;
        ef.UpdatedByUserId = certification.UpdatedByUserId;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SoftDeleteCertificationAsync(Guid id, Guid organizationId, Guid deletedByUserId)
    {
        var ef = await _context.Certifications
            .FirstOrDefaultAsync(c => c.Id == id && c.OrganizationId == organizationId && !c.IsDeleted);

        if (ef == null) return false;

        ef.IsDeleted = true;
        ef.DeletedAt = DateTime.UtcNow;
        ef.DeletedByUserId = deletedByUserId;

        await _context.SaveChangesAsync();
        return true;
    }

    private static DomainEntities.Certification MapToDomain(EfEntities.Certification ef)
    {
        return new DomainEntities.Certification
        {
            Id = ef.Id,
            UserId = ef.UserId,
            OrganizationId = ef.OrganizationId,
            Name = ef.Name,
            Issuer = ef.Issuer,
            IssueDate = ef.IssueDate,
            ExpirationDate = ef.ExpirationDate,
            EvidenceUrl = ef.EvidenceUrl,
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