using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Data.Entities;

[PrimaryKey("UserId", "PermissionId")]
[Table("UserPermissions", Schema = "iam")]
[Index("OrganizationId", "UserId", Name = "IX_UserPermissions_Org_User")]
public partial class UserPermission
{
    [Key]
    public Guid UserId { get; set; }

    [Key]
    public Guid PermissionId { get; set; }

    public Guid OrganizationId { get; set; }

    public bool IsGranted { get; set; }

    [Precision(3)]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("UserPermissions")]
    public virtual User User { get; set; } = null!;

    [ForeignKey("PermissionId")]
    [InverseProperty("UserPermissions")]
    public virtual Permission Permission { get; set; } = null!;

    [ForeignKey("OrganizationId")]
    [InverseProperty("UserPermissions")]
    public virtual Organization Organization { get; set; } = null!;
}