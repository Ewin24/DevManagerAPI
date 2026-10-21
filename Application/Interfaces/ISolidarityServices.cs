namespace Application.Interfaces;

using Application.DTOs.Solidarity;
using Domain.Enums;

/// <summary>
/// Servicio de asociados: alta, ciclo de vida e historial de membresía
/// </summary>
public interface IAssociateService
{
    Task<IEnumerable<AssociateDto>> GetAllAsync(Guid organizationId, AssociateStatus? status);
    Task<AssociateDetailDto> GetByIdAsync(Guid userId, Guid organizationId);
    Task<AssociateDetailDto> CreateAsync(CreateAssociateRequest request, Guid organizationId, Guid createdByUserId);
    Task<AssociateTransitionResponse> TransitionAsync(Guid userId, TransitionAssociateRequest request, Guid organizationId, Guid updatedByUserId);
}

/// <summary>
/// Servicio de políticas de datos (Habeas Data) y consentimientos
/// </summary>
public interface IConsentService
{
    Task<IEnumerable<DataPolicyDto>> GetPoliciesAsync(Guid organizationId);
    Task<DataPolicyDto?> GetCurrentPolicyAsync(Guid organizationId);
    Task<DataPolicyDto> CreatePolicyAsync(CreateDataPolicyRequest request, Guid organizationId, Guid createdByUserId);

    /// <summary>Las políticas publicadas son inmutables: siempre rechaza (409); se requiere una nueva versión</summary>
    Task UpdatePolicyAsync(Guid policyId, Guid organizationId);

    Task<IEnumerable<ConsentDto>> GetUserConsentsAsync(Guid userId, Guid organizationId);
    Task<ConsentDto> GrantAsync(Guid userId, GrantConsentRequest request, Guid organizationId, Guid grantedByUserId);
    Task<ConsentDto> RevokeAsync(Guid consentId, RevokeConsentRequest request, Guid organizationId, Guid revokedByUserId);
}
