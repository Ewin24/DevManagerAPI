namespace Domain.Entities.HabeasData;

using Domain.Common;
using Domain.Entities.IAM;

/// <summary>
/// Autorización de tratamiento de datos personales según Ley 1581/2012
/// Captura el consentimiento expreso del titular para el tratamiento de sus datos
/// </summary>
public class Autorizacion : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid AsociadoId { get; set; }
    public Guid OrganizationId { get; set; }

    /// <summary>Fecha en que se otorgó la autorización</summary>
    public DateTime FechaAutorizacion { get; set; }

    /// <summary>Vigencia de la autorización (indefinida o hasta fecha)</summary>
    public DateTime? Vigencia { get; set; }

    /// <summary>¿Ha sido revocada la autorización por el titular?</summary>
    public bool Revocada { get; set; }

    /// <summary>Fecha de revocación (si aplica)</summary>
    public DateTime? FechaRevocacion { get; set; }

    /// <summary>Finalidad del tratamiento de datos</summary>
    public string Finalidad { get; set; } = null!;

    /// <summary>Medio por el cual se otorgó la autorización (físico, digital)</summary>
    public string MedioAutorizacion { get; set; } = "Digital";

    /// <summary>IP o referencia del dispositivo desde donde se autorizó</summary>
    public string? DireccionIp { get; set; }

    // Navegación
    public User? Asociado { get; set; }
}
