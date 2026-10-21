namespace Domain.Entities.Solidarity;

using Domain.Common;

/// <summary>
/// Versión publicada de la política de tratamiento de datos (el texto publicado no cambia)
/// </summary>
public class DataPolicyVersion : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public string Version { get; set; } = null!;
    public string Purpose { get; set; } = null!;
    public string Text { get; set; } = null!;
    public DateOnly EffectiveFrom { get; set; }
}

/// <summary>
/// Consentimiento de tratamiento de datos. Solo se agrega; la revocación conserva el registro.
/// </summary>
public class DataConsent
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Guid UserId { get; set; }
    public Guid PolicyVersionId { get; set; }
    public DateTime GrantedAt { get; set; }
    public Guid GrantedByUserId { get; set; }
    public DateTime? RevokedAt { get; set; }
    public Guid? RevokedByUserId { get; set; }
    public string? RevocationReason { get; set; }

    public bool IsActive => RevokedAt == null;

    // Navegación
    public DataPolicyVersion? PolicyVersion { get; set; }
}
