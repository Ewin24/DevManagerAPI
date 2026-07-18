using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Data.Entities;

[Table("PersonOrganizations", Schema = "iam")]
public partial class PersonOrganization
{
    [Key]
    public Guid PersonId { get; set; }

    [Key]
    public Guid OrganizationId { get; set; }

    [StringLength(50)]
    public string MembershipType { get; set; } = null!;

    [Precision(3)]
    public DateTime JoinedAt { get; set; }

    public MembershipStatus Status { get; set; }

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

    [ForeignKey("PersonId")]
    [InverseProperty("PersonOrganizations")]
    public virtual User Person { get; set; } = null!;

    [ForeignKey("OrganizationId")]
    [InverseProperty("PersonOrganizations")]
    public virtual Organization Organization { get; set; } = null!;
}
