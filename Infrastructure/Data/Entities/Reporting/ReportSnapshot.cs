using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Entities;

[Table("ReportSnapshots", Schema = "reporting")]
[Index("OrganizationId", "SnapshotDate", Name = "UX_ReportSnapshots_Org_Date", IsUnique = true)]
public partial class ReportSnapshot
{
    [Key]
    public Guid Id { get; set; }

    public Guid OrganizationId { get; set; }

    public DateOnly SnapshotDate { get; set; }

    public string JsonPayload { get; set; } = null!;

    [Precision(3)]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("OrganizationId")]
    [InverseProperty("ReportSnapshots")]
    public virtual Organization Organization { get; set; } = null!;
}
