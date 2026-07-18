namespace Domain.Entities.IAM;

using Domain.Common;
using Domain.Enums;

/// <summary>
/// Relación muchos-a-muchos entre una Persona (User con PersonType)
/// y una Organización solidaria, con metadatos de membresía
/// </summary>
public class PersonOrganization : AuditableEntity
{
    public Guid PersonId { get; set; }
    public Guid OrganizationId { get; set; }

    /// <summary>Tipo de membresía o vinculación</summary>
    public string MembershipType { get; set; } = null!;

    /// <summary>Fecha de ingreso a la organización</summary>
    public DateTime JoinedAt { get; set; }

    /// <summary>Estado actual de la membresía</summary>
    public MembershipStatus Status { get; set; }

    // Navegación
    public User Person { get; set; } = null!;
    public Organization Organization { get; set; } = null!;
}
