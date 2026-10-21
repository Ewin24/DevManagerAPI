namespace Application.Services;

using Application.Common.Exceptions;
using Application.DTOs.Solidarity;
using Application.Interfaces;
using Domain.Entities.Solidarity;
using Domain.Enums;
using Domain.Interfaces.Repositories;

/// <summary>
/// Servicio de asociados. Todas las operaciones se resuelven dentro de la organización del llamante.
/// </summary>
public class AssociateService : IAssociateService
{
    private const int MaxReasonLength = 400;

    private readonly IAssociateRepository _associateRepository;
    private readonly IUserRepository _userRepository;
    private readonly IConsentRepository _consentRepository;

    public AssociateService(
        IAssociateRepository associateRepository,
        IUserRepository userRepository,
        IConsentRepository consentRepository)
    {
        _associateRepository = associateRepository;
        _userRepository = userRepository;
        _consentRepository = consentRepository;
    }

    public async Task<IEnumerable<AssociateDto>> GetAllAsync(Guid organizationId, AssociateStatus? status)
    {
        var associates = await _associateRepository.GetAllAsync(organizationId, status);
        return associates.Select(a => MapToDto(a, new AssociateDto())).ToList();
    }

    public async Task<AssociateDetailDto> GetByIdAsync(Guid userId, Guid organizationId)
    {
        var associate = await _associateRepository.GetByUserIdAsync(userId, organizationId)
            ?? throw new NotFoundException("Asociado", userId);

        var detail = MapToDto(associate, new AssociateDetailDto());
        detail.Email = associate.User!.Email;
        detail.Periods = associate.Periods.Select(p => new MembershipPeriodDto
        {
            Id = p.Id,
            ApplicationDate = p.ApplicationDate,
            AdmissionDate = p.AdmissionDate,
            WithdrawalDate = p.WithdrawalDate,
            WithdrawalReason = p.WithdrawalReason
        }).ToList();
        detail.HasActiveConsent = await _consentRepository.HasActiveConsentAsync(userId, organizationId);
        return detail;
    }

    public async Task<AssociateDetailDto> CreateAsync(CreateAssociateRequest request, Guid organizationId, Guid createdByUserId)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, organizationId)
            ?? throw new NotFoundException("Usuario", request.UserId);

        if (await _associateRepository.GetByUserIdAsync(user.Id, organizationId) != null)
        {
            throw new ConflictException("El usuario ya tiene un registro de asociado; use la transición de reingreso si fue retirado");
        }

        var now = DateTime.UtcNow;
        var associate = new Associate
        {
            UserId = user.Id,
            OrganizationId = organizationId,
            Status = AssociateStatus.Applicant,
            CreatedAt = now,
            CreatedByUserId = createdByUserId,
            Periods = { NewPeriod(user.Id, organizationId, request.ApplicationDate ?? Today(), now, createdByUserId) }
        };

        await _associateRepository.CreateAsync(associate);
        return await GetByIdAsync(user.Id, organizationId);
    }

    public async Task<AssociateTransitionResponse> TransitionAsync(
        Guid userId,
        TransitionAssociateRequest request,
        Guid organizationId,
        Guid updatedByUserId)
    {
        var associate = await _associateRepository.GetByUserIdAsync(userId, organizationId)
            ?? throw new NotFoundException("Asociado", userId);

        var from = associate.Status;
        var to = request.ToStatus;
        var date = request.EffectiveDate ?? Today();

        if (!Enum.IsDefined(to))
            throw new BusinessValidationException("Estado destino inválido");

        if (!AssociateLifecycle.CanTransition(from, to))
        {
            throw new ConflictException($"Transición no permitida: {from} -> {to}");
        }

        var warnings = new List<string>();
        var open = associate.OpenPeriod;

        if (to == AssociateStatus.Active && from == AssociateStatus.Applicant)
        {
            if (open == null || date < open.ApplicationDate)
                throw new BusinessValidationException("La fecha de admisión no puede ser anterior a la fecha de solicitud");
            open.AdmissionDate = date;
        }
        else if (to == AssociateStatus.Withdrawn)
        {
            var reason = request.Reason?.Trim();
            if (string.IsNullOrEmpty(reason))
                throw new BusinessValidationException("El motivo de retiro es obligatorio");
            if (reason.Length > MaxReasonLength)
                throw new BusinessValidationException($"El motivo no puede superar {MaxReasonLength} caracteres");
            if (open == null || date < (open.AdmissionDate ?? open.ApplicationDate))
                throw new BusinessValidationException("La fecha de retiro no puede ser anterior a la admisión o solicitud");

            open.WithdrawalDate = date;
            open.WithdrawalReason = reason;
        }
        else if (to == AssociateStatus.Applicant) // reingreso: abre un periodo nuevo sin tocar los anteriores
        {
            var lastWithdrawal = associate.Periods.Max(p => p.WithdrawalDate);
            if (lastWithdrawal.HasValue && date < lastWithdrawal.Value)
                throw new BusinessValidationException("La fecha de reingreso no puede ser anterior al último retiro");

            associate.Periods.Add(NewPeriod(userId, organizationId, date, DateTime.UtcNow, updatedByUserId));
        }

        associate.Status = to;
        associate.UpdatedByUserId = updatedByUserId;
        await _associateRepository.UpdateAsync(associate);

        return new AssociateTransitionResponse { Status = to, Warnings = warnings };
    }

    #region Helpers

    private static DateOnly Today() => DateOnly.FromDateTime(DateTime.UtcNow);

    private static MembershipPeriod NewPeriod(Guid userId, Guid organizationId, DateOnly applicationDate, DateTime now, Guid createdBy) => new()
    {
        Id = Guid.NewGuid(),
        OrganizationId = organizationId,
        UserId = userId,
        ApplicationDate = applicationDate,
        CreatedAt = now,
        CreatedByUserId = createdBy
    };

    private static T MapToDto<T>(Associate associate, T dto) where T : AssociateDto
    {
        var user = associate.User!;
        dto.UserId = associate.UserId;
        dto.FullName = $"{user.FirstName} {user.LastName}".Trim();
        dto.DocumentType = user.DocumentType;
        dto.DocumentNumber = user.DocumentNumber;
        dto.Status = associate.Status;
        dto.IsEmployee = user.IsEmployee;
        dto.Role = AssociateLifecycle.ResolveRole(user.IsEmployee, associate.Status).ToString();
        dto.CurrentAdmissionDate = associate.OpenPeriod?.AdmissionDate;
        return dto;
    }

    #endregion
}
