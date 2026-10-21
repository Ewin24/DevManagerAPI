using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Data.Entities;

[Table("EducationPrograms", Schema = "solidarity")]
[Index("OrganizationId", Name = "IX_EducationPrograms_Org")]
public partial class EducationProgram
{
    [Key]
    public Guid Id { get; set; }

    public Guid OrganizationId { get; set; }

    [StringLength(160)]
    public string Name { get; set; } = null!;

    public byte Type { get; set; }

    [Precision(5, 1)]
    public decimal RequiredHours { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    [Precision(3)]
    public DateTime CreatedAt { get; set; }

    public Guid? CreatedByUserId { get; set; }

    [Precision(3)]
    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedByUserId { get; set; }

    public bool IsDeleted { get; set; }

    [Precision(3)]
    public DateTime? DeletedAt { get; set; }

    public Guid? DeletedByUserId { get; set; }

    [ForeignKey("OrganizationId")]
    public virtual Organization Organization { get; set; } = null!;
}

[Table("EducationEnrollments", Schema = "solidarity")]
[Index("OrganizationId", "UserId", Name = "IX_EducationEnrollments_Org_User")]
public partial class EducationEnrollment
{
    [Key]
    public Guid Id { get; set; }

    public Guid OrganizationId { get; set; }

    public Guid ProgramId { get; set; }

    public Guid UserId { get; set; }

    public byte Status { get; set; }

    [Precision(5, 1)]
    public decimal HoursCompleted { get; set; }

    public DateOnly? CompletionDate { get; set; }

    [Precision(3)]
    public DateTime CreatedAt { get; set; }

    public Guid? CreatedByUserId { get; set; }

    [Precision(3)]
    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedByUserId { get; set; }

    public bool IsDeleted { get; set; }

    [Precision(3)]
    public DateTime? DeletedAt { get; set; }

    public Guid? DeletedByUserId { get; set; }

    [ForeignKey("ProgramId")]
    public virtual EducationProgram Program { get; set; } = null!;

    [ForeignKey("UserId")]
    public virtual Associate Associate { get; set; } = null!;
}
