namespace Domain.Entities.Solidarity;

using Domain.Common;
using Domain.Entities.IAM;
using Domain.Enums;

/// <summary>
/// Asociado de la organización (a lo sumo uno por usuario). Conserva el estado actual;
/// cada membresía vive en <see cref="MembershipPeriod"/>.
/// </summary>
public class Associate : AuditableEntity
{
    public Guid UserId { get; set; }
    public Guid OrganizationId { get; set; }
    public AssociateStatus Status { get; set; }

    // Navegación
    public User? User { get; set; }
    public List<MembershipPeriod> Periods { get; set; } = new();

    /// <summary>Periodo de membresía abierto (sin fecha de retiro), si existe</summary>
    public MembershipPeriod? OpenPeriod => Periods.FirstOrDefault(p => p.WithdrawalDate == null);
}

/// <summary>
/// Una membresía: solicitud, admisión, retiro y motivo. El reingreso abre un periodo nuevo.
/// </summary>
public class MembershipPeriod : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Guid UserId { get; set; }
    public DateOnly ApplicationDate { get; set; }
    public DateOnly? AdmissionDate { get; set; }
    public DateOnly? WithdrawalDate { get; set; }
    public string? WithdrawalReason { get; set; }
}
