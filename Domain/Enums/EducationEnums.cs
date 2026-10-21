namespace Domain.Enums;

/// <summary>
/// Type of cooperative education program
/// </summary>
public enum EducationProgramType : byte
{
    Basic = 1,
    Permanent = 2,
    Governance = 3,
    Informal = 4
}

/// <summary>
/// Status of an associate's enrollment in a program
/// </summary>
public enum EnrollmentStatus : byte
{
    Enrolled = 1,
    Completed = 2,
    Dropped = 3
}
