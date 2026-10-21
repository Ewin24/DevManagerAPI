using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Data.Entities;

[Table("DataPolicyVersions", Schema = "solidarity")]
public partial class DataPolicyVersion
{
    [Key]
    public Guid Id { get; set; }

    public Guid OrganizationId { get; set; }

    [StringLength(20)]
    public string Version { get; set; } = null!;

    [StringLength(300)]
    public string Purpose { get; set; } = null!;

    public string Text { get; set; } = null!;

    public DateOnly EffectiveFrom { get; set; }

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
}

[Table("DataConsents", Schema = "solidarity")]
public partial class DataConsent
{
    [Key]
    public Guid Id { get; set; }

    public Guid OrganizationId { get; set; }

    public Guid UserId { get; set; }

    public Guid PolicyVersionId { get; set; }

    [Precision(3)]
    public DateTime GrantedAt { get; set; }

    public Guid GrantedByUserId { get; set; }

    [Precision(3)]
    public DateTime? RevokedAt { get; set; }

    public Guid? RevokedByUserId { get; set; }

    [StringLength(400)]
    public string? RevocationReason { get; set; }

    [Precision(3)]
    public DateTime CreatedAt { get; set; }

    public Guid? CreatedByUserId { get; set; }

    [ForeignKey("PolicyVersionId")]
    public virtual DataPolicyVersion PolicyVersion { get; set; } = null!;
}
