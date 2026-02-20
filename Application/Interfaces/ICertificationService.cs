namespace Application.Interfaces;

using Application.DTOs.Profiles;

/// <summary>
/// Servicio para gestión de certificaciones de empleados
/// </summary>
public interface ICertificationService
{
    Task<IEnumerable<CertificationDto>> GetMyCertificationsAsync(Guid userId, Guid organizationId);
    Task<CertificationDto?> GetCertificationByIdAsync(Guid certificationId, Guid organizationId);
    Task<Guid> CreateCertificationAsync(Guid userId, Guid organizationId, CertificationRequest request);
    Task<bool> UpdateCertificationAsync(Guid certificationId, Guid userId, Guid organizationId, CertificationRequest request);
    Task<bool> DeleteCertificationAsync(Guid certificationId, Guid userId, Guid organizationId);
    
    // métodos internos para agente
    Task<IEnumerable<CertificationDto>> GetCertificationsByUserIdAsync(Guid userId, Guid organizationId);
}