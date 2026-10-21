namespace Domain.Entities.Solidarity;

using Domain.Enums;

/// <summary>
/// Calendar-year education coverage KPI (pure, no I/O).
/// Covered associate = has a Completed enrollment whose completion date falls in the year.
/// Average hours = completed hours in the year / associates active at any point in the year.
/// </summary>
public static class EducationKpiCalculator
{
    /// <summary>Hours of completed basic education required before admission (warning only)</summary>
    public const decimal BasicEducationMinimumHours = 20m;

    public static EducationCoverage Compute(
        IEnumerable<MembershipPeriod> periods,
        IEnumerable<EducationEnrollment> enrollments,
        int year)
    {
        var yearStart = new DateOnly(year, 1, 1);
        var yearEnd = new DateOnly(year, 12, 31);

        // Active at any time in the year: admitted on/before Dec 31 and not withdrawn before Jan 1
        var eligible = periods
            .Where(p => p.AdmissionDate.HasValue
                        && p.AdmissionDate.Value <= yearEnd
                        && (p.WithdrawalDate == null || p.WithdrawalDate.Value >= yearStart))
            .Select(p => p.UserId)
            .ToHashSet();

        var all = enrollments.Where(e => e.Status == EnrollmentStatus.Completed).ToList();

        var completedInYear = all
            .Where(e => eligible.Contains(e.UserId) && e.CompletionDate?.Year == year)
            .ToList();

        var covered = completedInYear.Select(e => e.UserId).Distinct().Count();
        var totalHours = completedInYear.Sum(e => e.HoursCompleted);

        var basicHours = all
            .Where(e => e.ProgramType == EducationProgramType.Basic)
            .GroupBy(e => e.UserId)
            .ToDictionary(g => g.Key, g => g.Sum(e => e.HoursCompleted));

        return new EducationCoverage
        {
            Year = year,
            EligibleAssociates = eligible.Count,
            CoveredAssociates = covered,
            CoverageRatio = eligible.Count == 0 ? 0 : Math.Round((decimal)covered / eligible.Count, 4),
            AvgHours = eligible.Count == 0 ? 0 : Math.Round(totalHours / eligible.Count, 2),
            BasicCompliant = eligible.Count > 0
                && eligible.All(u => basicHours.GetValueOrDefault(u) >= BasicEducationMinimumHours),
            HoursByType = completedInYear
                .GroupBy(e => e.ProgramType.ToString())
                .ToDictionary(g => g.Key, g => g.Sum(e => e.HoursCompleted))
        };
    }
}

public class EducationCoverage
{
    public int Year { get; set; }
    public int EligibleAssociates { get; set; }
    public int CoveredAssociates { get; set; }
    public decimal CoverageRatio { get; set; }
    public decimal AvgHours { get; set; }
    public bool BasicCompliant { get; set; }
    public Dictionary<string, decimal> HoursByType { get; set; } = new();
}
