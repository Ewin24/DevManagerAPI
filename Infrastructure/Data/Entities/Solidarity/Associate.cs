using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Data.Entities;

[Table("Associates", Schema = "solidarity")]
[Index("OrganizationId", "Status", Name = "IX_Associates_Org_Status")]
public partial class Associate
{
    [Key]
    public Guid UserId { get; set; }

    public Guid OrganizationId { get; set; }

    public byte Status { get; set; }

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

    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;

    [ForeignKey("OrganizationId")]
    public virtual Organization Organization { get; set; } = null!;

    public virtual ICollection<MembershipPeriod> MembershipPeriods { get; set; } = new List<MembershipPeriod>();
}

[Table("MembershipPeriods", Schema = "solidarity")]
public partial class MembershipPeriod
{
    [Key]
    public Guid Id { get; set; }

    public Guid OrganizationId { get; set; }

    public Guid UserId { get; set; }

    public DateOnly ApplicationDate { get; set; }

    public DateOnly? AdmissionDate { get; set; }

    public DateOnly? WithdrawalDate { get; set; }

    [StringLength(400)]
    public string? WithdrawalReason { get; set; }

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

    [ForeignKey("UserId")]
    public virtual Associate Associate { get; set; } = null!;
}
