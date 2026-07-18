namespace Domain.Entities.IAM;

using Domain.Common;

/// <summary>
/// Representa una organización/empresa cliente (raíz multi-tenant)
/// </summary>
public class Organization : AuditableEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? LegalName { get; set; }
    public string? Nit { get; set; }
    public bool IsActive { get; set; }
}
