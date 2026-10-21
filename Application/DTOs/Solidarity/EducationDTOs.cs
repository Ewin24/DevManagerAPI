namespace Application.DTOs.Solidarity;

using Domain.Enums;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// DTO to create or update an education program
/// </summary>
public class SaveEducationProgramRequest
{
    [Required]
    [MaxLength(160)]
    public string Name { get; set; } = null!;

    /// <summary>1 Basic, 2 Permanent, 3 Governance, 4 Informal</summary>
    public EducationProgramType Type { get; set; }

    /// <summary>Must be greater than 0</summary>
    public decimal RequiredHours { get; set; }

    /// <summary>Defaults to today (UTC)</summary>
    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }
}

public class EducationProgramDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public EducationProgramType Type { get; set; }
    public decimal RequiredHours { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
}

/// <summary>
/// DTO to enroll an associate in a program
/// </summary>
public class CreateEnrollmentRequest
{
    [Required]
    public Guid UserId { get; set; }

    /// <summary>Defaults to Enrolled (1)</summary>
    public EnrollmentStatus? Status { get; set; }

    public decimal HoursCompleted { get; set; }

    /// <summary>Required when Status is Completed (2)</summary>
    public DateOnly? CompletionDate { get; set; }
}

/// <summary>
/// DTO to update an enrollment's status and hours
/// </summary>
public class UpdateEnrollmentRequest
{
    /// <summary>1 Enrolled, 2 Completed, 3 Dropped</summary>
    public EnrollmentStatus Status { get; set; }

    public decimal HoursCompleted { get; set; }

    /// <summary>Required when Status is Completed (2); ignored otherwise</summary>
    public DateOnly? CompletionDate { get; set; }
}

public class EnrollmentDto
{
    public Guid Id { get; set; }
    public Guid ProgramId { get; set; }
    public Guid UserId { get; set; }
    public string FullName { get; set; } = null!;
    public EnrollmentStatus Status { get; set; }
    public decimal HoursCompleted { get; set; }
    public DateOnly? CompletionDate { get; set; }
}
