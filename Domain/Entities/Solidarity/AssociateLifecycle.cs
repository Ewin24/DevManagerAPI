namespace Domain.Entities.Solidarity;

using Domain.Enums;

/// <summary>
/// Única fuente de verdad de las transiciones del asociado y del rol derivado
/// </summary>
public static class AssociateLifecycle
{
    private static readonly HashSet<(AssociateStatus From, AssociateStatus To)> Transitions = new()
    {
        (AssociateStatus.Applicant, AssociateStatus.Active),
        (AssociateStatus.Applicant, AssociateStatus.Withdrawn),
        (AssociateStatus.Active, AssociateStatus.Suspended),
        (AssociateStatus.Suspended, AssociateStatus.Active),
        (AssociateStatus.Active, AssociateStatus.Withdrawn),
        (AssociateStatus.Suspended, AssociateStatus.Withdrawn),
        (AssociateStatus.Withdrawn, AssociateStatus.Applicant) // reingreso
    };

    public static bool CanTransition(AssociateStatus from, AssociateStatus to) => Transitions.Contains((from, to));

    /// <summary>
    /// Asociado = registro no retirado; empleado = bandera explícita del usuario
    /// </summary>
    public static AssociateRole ResolveRole(bool isEmployee, AssociateStatus? status)
    {
        var isAssociate = status.HasValue && status.Value != AssociateStatus.Withdrawn;
        return (isAssociate, isEmployee) switch
        {
            (true, true) => AssociateRole.Both,
            (true, false) => AssociateRole.Associate,
            (false, true) => AssociateRole.Employee,
            _ => AssociateRole.None
        };
    }
}
