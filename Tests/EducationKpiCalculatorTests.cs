using Xunit;
using Domain.Entities.Solidarity;
using Domain.Enums;

namespace Tests;

/// <summary>
/// Pure unit tests for the calendar-year education coverage KPI
/// </summary>
public class EducationKpiCalculatorTests
{
    private static MembershipPeriod Period(Guid user, string admitted, string? withdrawn = null) => new()
    {
        UserId = user,
        ApplicationDate = DateOnly.Parse(admitted),
        AdmissionDate = DateOnly.Parse(admitted),
        WithdrawalDate = withdrawn == null ? null : DateOnly.Parse(withdrawn)
    };

    private static EducationEnrollment Completed(Guid user, decimal hours, string date,
        EducationProgramType type = EducationProgramType.Permanent) => new()
    {
        UserId = user,
        Status = EnrollmentStatus.Completed,
        HoursCompleted = hours,
        CompletionDate = DateOnly.Parse(date),
        ProgramType = type
    };

    [Fact]
    public void SpecScenario_TenActive_FourCompleting60Hours_Gives04And6()
    {
        var users = Enumerable.Range(0, 10).Select(_ => Guid.NewGuid()).ToList();
        var periods = users.Select(u => Period(u, "2025-01-01"));
        var enrollments = users.Take(4).Select(u => Completed(u, 15, "2026-05-01"));

        var kpi = EducationKpiCalculator.Compute(periods, enrollments, 2026);

        Assert.Equal(10, kpi.EligibleAssociates);
        Assert.Equal(4, kpi.CoveredAssociates);
        Assert.Equal(0.4m, kpi.CoverageRatio);
        Assert.Equal(6.0m, kpi.AvgHours);
        Assert.Equal(60m, kpi.HoursByType["Permanent"]);
    }

    [Fact]
    public void CompletionOnPriorYearLastDay_IsExcluded()
    {
        var u = Guid.NewGuid();

        var kpi = EducationKpiCalculator.Compute(
            new[] { Period(u, "2025-01-01") }, new[] { Completed(u, 10, "2025-12-31") }, 2026);

        Assert.Equal(0, kpi.CoveredAssociates);
        Assert.Equal(0m, kpi.AvgHours);
    }

    [Fact]
    public void EnrolledOrDropped_DoNotCount()
    {
        var u = Guid.NewGuid();
        var enrolled = Completed(u, 10, "2026-02-01");
        enrolled.Status = EnrollmentStatus.Enrolled;

        var kpi = EducationKpiCalculator.Compute(new[] { Period(u, "2025-01-01") }, new[] { enrolled }, 2026);

        Assert.Equal(0, kpi.CoveredAssociates);
    }

    [Fact]
    public void ZeroActiveAssociates_YieldsZeroRatioWithoutError()
    {
        var kpi = EducationKpiCalculator.Compute(Array.Empty<MembershipPeriod>(), Array.Empty<EducationEnrollment>(), 2026);

        Assert.Equal(0, kpi.EligibleAssociates);
        Assert.Equal(0m, kpi.CoverageRatio);
        Assert.Equal(0m, kpi.AvgHours);
        Assert.False(kpi.BasicCompliant);
    }

    [Fact]
    public void Readmission_WithOverlappingYearPeriods_CountsAssociateOnce()
    {
        var u = Guid.NewGuid();
        var periods = new[] { Period(u, "2025-01-01", "2026-03-01"), Period(u, "2026-06-01") };

        var kpi = EducationKpiCalculator.Compute(periods, new[] { Completed(u, 8, "2026-07-01") }, 2026);

        Assert.Equal(1, kpi.EligibleAssociates);
        Assert.Equal(1, kpi.CoveredAssociates);
        Assert.Equal(1m, kpi.CoverageRatio);
    }

    [Fact]
    public void AssociateWithdrawnBeforeYearOrAdmittedAfter_IsNotEligible()
    {
        var before = Guid.NewGuid();
        var after = Guid.NewGuid();
        var periods = new[] { Period(before, "2024-01-01", "2025-12-31"), Period(after, "2027-01-01") };

        var kpi = EducationKpiCalculator.Compute(periods, Array.Empty<EducationEnrollment>(), 2026);

        Assert.Equal(0, kpi.EligibleAssociates);
    }

    [Fact]
    public void BasicCompliant_RequiresEveryEligibleAssociateAtTwentyHours()
    {
        var a = Guid.NewGuid();
        var b = Guid.NewGuid();
        var periods = new[] { Period(a, "2025-01-01"), Period(b, "2025-01-01") };
        var enrollments = new[]
        {
            Completed(a, 20, "2025-03-01", EducationProgramType.Basic),
            Completed(b, 12, "2025-03-01", EducationProgramType.Basic)
        };

        Assert.False(EducationKpiCalculator.Compute(periods, enrollments, 2026).BasicCompliant);

        enrollments[1].HoursCompleted = 20;
        Assert.True(EducationKpiCalculator.Compute(periods, enrollments, 2026).BasicCompliant);
    }
}
