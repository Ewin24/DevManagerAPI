namespace Domain.Entities.Solidarity;

using Domain.Common;
using Domain.Enums;

/// <summary>
/// Cooperative education program (basic, permanent, governance or informal)
/// </summary>
public class EducationProgram : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public string Name { get; set; } = null!;
    public EducationProgramType Type { get; set; }
    public decimal RequiredHours { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
}

/// <summary>
/// An associate's enrollment in a program (at most one per program and associate)
/// </summary>
public class EducationEnrollment : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Guid ProgramId { get; set; }
    public Guid UserId { get; set; }
    public EnrollmentStatus Status { get; set; }
    public decimal HoursCompleted { get; set; }
    public DateOnly? CompletionDate { get; set; }

    // Navigation (populated by the repository)
    public EducationProgramType ProgramType { get; set; }
    public string? FullName { get; set; }
}
