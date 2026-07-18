using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Data.Entities;

[Table("Permissions", Schema = "iam")]
[Index("Code", Name = "UX_Permissions_Code", IsUnique = true)]
public partial class Permission
{
    [Key]
    public Guid Id { get; set; }

    [StringLength(100)]
    public string Code { get; set; } = null!;

    [StringLength(160)]
    public string Name { get; set; } = null!;

    [StringLength(400)]
    public string? Description { get; set; }

    [StringLength(80)]
    public string Module { get; set; } = null!;

    [Precision(3)]
    public DateTime CreatedAt { get; set; }

    public bool IsDeleted { get; set; }

    [InverseProperty("Permission")]
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

    [InverseProperty("Permission")]
    public virtual ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
}