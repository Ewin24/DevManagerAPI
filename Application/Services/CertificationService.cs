namespace Application.Services;

using Application.DTOs.Profiles;
using Application.Interfaces;
using Domain.Entities.Talent;
using Domain.Interfaces.Repositories;

/// <summary>
/// Servicio para gestión de certificaciones
/// </summary>
public class CertificationService : ICertificationService
{
    private readonly ICertificationRepository _certRepository;

    public CertificationService(ICertificationRepository certRepository)
    {
        _certRepository = certRepository;
    }

    public async Task<IEnumerable<CertificationDto>> GetMyCertificationsAsync(Guid userId, Guid organizationId)
    {
        var certs = await _certRepository.GetCertificationsByUserIdAsync(userId, organizationId);
        return certs.Select(Map);
    }

    public async Task<CertificationDto?> GetCertificationByIdAsync(Guid certificationId, Guid organizationId)
    {
        var cert = await _certRepository.GetCertificationByIdAsync(certificationId);
        if (cert == null || cert.OrganizationId != organizationId)
            return null;
        return Map(cert);
    }

    public async Task<Guid> CreateCertificationAsync(Guid userId, Guid organizationId, CertificationRequest request)
    {
        var entity = new Certification
        {
            OrganizationId = organizationId,
            UserId = userId,
            Name = request.Name,
            Issuer = request.Issuer,
            IssueDate = request.IssueDate,
            ExpirationDate = request.ExpirationDate,
            EvidenceUrl = request.EvidenceUrl,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = userId
        };

        return await _certRepository.CreateCertificationAsync(entity);
    }

    public async Task<bool> UpdateCertificationAsync(Guid certificationId, Guid userId, Guid organizationId, CertificationRequest request)
    {
        var cert = await _certRepository.GetCertificationByIdAsync(certificationId);
        if (cert == null || cert.OrganizationId != organizationId || cert.UserId != userId)
            return false;

        cert.Name = request.Name;
        cert.Issuer = request.Issuer;
        cert.IssueDate = request.IssueDate;
        cert.ExpirationDate = request.ExpirationDate;
        cert.EvidenceUrl = request.EvidenceUrl;
        cert.UpdatedAt = DateTime.UtcNow;
        cert.UpdatedByUserId = userId;

        return await _certRepository.UpdateCertificationAsync(cert);
    }

    public async Task<bool> DeleteCertificationAsync(Guid certificationId, Guid userId, Guid organizationId)
    {
        var cert = await _certRepository.GetCertificationByIdAsync(certificationId);
        if (cert == null || cert.OrganizationId != organizationId || cert.UserId != userId)
            return false;

        return await _certRepository.SoftDeleteCertificationAsync(certificationId, organizationId, userId);
    }

    // internal/agent
    public async Task<IEnumerable<CertificationDto>> GetCertificationsByUserIdAsync(Guid userId, Guid organizationId)
    {
        var certs = await _certRepository.GetCertificationsByUserIdAsync(userId, organizationId);
        return certs.Select(Map);
    }

    private CertificationDto Map(Certification c) => new CertificationDto
    {
        Id = c.Id,
        UserId = c.UserId,
        Name = c.Name,
        Issuer = c.Issuer,
        IssueDate = c.IssueDate,
        ExpirationDate = c.ExpirationDate,
        EvidenceUrl = c.EvidenceUrl
    };
}