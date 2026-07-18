namespace Domain.Entities.IAM;

using Domain.Common;
using Domain.Enums;

/// <summary>
/// Representa una organización/empresa cliente (raíz multi-tenant)
/// Extiende con jerarquía y tipo solidario
/// </summary>
public class Organization : AuditableEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? LegalName { get; set; }
    public string? Nit { get; set; }
    public bool IsActive { get; set; }

    // ===== Jerarquía solidaria =====
    /// <summary>Tipo de organización solidaria</summary>
    public TipoOrganizacionSolidaria? TipoOrganizacionSolidaria { get; set; }

    /// <summary>Organización padre en la jerarquía (nullable para raíz)</summary>
    public Guid? ParentId { get; set; }

    /// <summary>Nivel jerárquico: 0 = raíz, 1+ = hijo</summary>
    public int HierarchyLevel { get; set; }

    // Navegación
    public ICollection<User> Users { get; set; } = new List<User>();

    /// <summary>Organizaciones hijas en la jerarquía</summary>
    public ICollection<Organization> Children { get; set; } = new List<Organization>();

    /// <summary>Organización padre</summary>
    public Organization? Parent { get; set; }

    /// <summary>Personas asociadas a esta organización</summary>
    public ICollection<PersonOrganization> PersonOrganizations { get; set; } = new List<PersonOrganization>();
}
