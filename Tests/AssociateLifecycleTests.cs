using Domain.Entities.Solidarity;
using Domain.Enums;
using Xunit;

namespace Tests;

public class AssociateLifecycleTests
{
    private static readonly HashSet<(AssociateStatus, AssociateStatus)> Allowed = new()
    {
        (AssociateStatus.Applicant, AssociateStatus.Active),
        (AssociateStatus.Applicant, AssociateStatus.Withdrawn),
        (AssociateStatus.Active, AssociateStatus.Suspended),
        (AssociateStatus.Suspended, AssociateStatus.Active),
        (AssociateStatus.Active, AssociateStatus.Withdrawn),
        (AssociateStatus.Suspended, AssociateStatus.Withdrawn),
        (AssociateStatus.Withdrawn, AssociateStatus.Applicant), // readmission
    };

    public static IEnumerable<object[]> AllPairs() =>
        from from_ in Enum.GetValues<AssociateStatus>()
        from to in Enum.GetValues<AssociateStatus>()
        select new object[] { from_, to };

    [Theory]
    [MemberData(nameof(AllPairs))]
    public void CanTransition_MatchesSpecTable(AssociateStatus from, AssociateStatus to)
        => Assert.Equal(Allowed.Contains((from, to)), AssociateLifecycle.CanTransition(from, to));

    [Theory]
    [InlineData(true, AssociateStatus.Active, AssociateRole.Both)]
    [InlineData(false, AssociateStatus.Suspended, AssociateRole.Associate)]
    [InlineData(true, AssociateStatus.Withdrawn, AssociateRole.Employee)]
    [InlineData(true, null, AssociateRole.Employee)]
    [InlineData(false, AssociateStatus.Withdrawn, AssociateRole.None)]
    [InlineData(false, null, AssociateRole.None)]
    public void ResolveRole_AssociateMeansNonWithdrawnRecord(bool isEmployee, AssociateStatus? status, AssociateRole expected)
        => Assert.Equal(expected, AssociateLifecycle.ResolveRole(isEmployee, status));
}
