namespace Application.DTOs.Solidarity;

using Domain.Enums;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// DTO para registrar un usuario como aspirante a asociado
/// </summary>
public class CreateAssociateRequest
{
    [Required]
    public Guid UserId { get; set; }

    /// <summary>Fecha de solicitud; por defecto hoy (UTC)</summary>
    public DateOnly? ApplicationDate { get; set; }
}

/// <summary>
/// DTO para cambiar el estado de un asociado
/// </summary>
public class TransitionAssociateRequest
{
    public AssociateStatus ToStatus { get; set; }

    /// <summary>Fecha efectiva de la transición; por defecto hoy (UTC)</summary>
    public DateOnly? EffectiveDate { get; set; }

    /// <summary>Obligatorio al retirar al asociado</summary>
    [MaxLength(400)]
    public string? Reason { get; set; }
}

public class MembershipPeriodDto
{
    public Guid Id { get; set; }
    public DateOnly ApplicationDate { get; set; }
    public DateOnly? AdmissionDate { get; set; }
    public DateOnly? WithdrawalDate { get; set; }
    public string? WithdrawalReason { get; set; }
}

public class AssociateDto
{
    public Guid UserId { get; set; }
    public string FullName { get; set; } = null!;
    public DocumentType? DocumentType { get; set; }
    public string? DocumentNumber { get; set; }
    public AssociateStatus Status { get; set; }
    public bool IsEmployee { get; set; }

    /// <summary>Associate, Employee, Both o None</summary>
    public string Role { get; set; } = null!;

    /// <summary>Fecha de admisión del periodo abierto (null si aspirante o retirado)</summary>
    public DateOnly? CurrentAdmissionDate { get; set; }
}

public class AssociateDetailDto : AssociateDto
{
    public string Email { get; set; } = null!;
    public List<MembershipPeriodDto> Periods { get; set; } = new();
    public bool HasActiveConsent { get; set; }
}

public class AssociateTransitionResponse
{
    public AssociateStatus Status { get; set; }
    public List<string> Warnings { get; set; } = new();
}
