namespace Domain.Common;

/// <summary>
/// Clase base para entidades con auditoría completa y soft delete
/// </summary>
public abstract class AuditableEntity
{
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedByUserId { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedByUserId { get; set; }
}
