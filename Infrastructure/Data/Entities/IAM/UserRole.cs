using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Data.Entities;

[PrimaryKey("UserId", "RoleId")]
[Table("UserRoles", Schema = "iam")]
[Index("OrganizationId", "UserId", Name = "IX_UserRoles_Org_User")]
public partial class UserRole
{
    [Key]
    public Guid UserId { get; set; }

    [Key]
    public Guid RoleId { get; set; }

    public Guid OrganizationId { get; set; }

    [Precision(3)]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("OrganizationId")]
    [InverseProperty("UserRoles")]
    public virtual Organization Organization { get; set; } = null!;

    [ForeignKey("RoleId")]
    [InverseProperty("UserRoles")]
    public virtual Role Role { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("UserRoles")]
    public virtual User User { get; set; } = null!;
}
