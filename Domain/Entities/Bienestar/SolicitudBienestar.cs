namespace Domain.Entities.Bienestar;

using Domain.Common;
using Domain.Entities.IAM;
using Domain.Enums;

/// <summary>
/// Solicitud de un asociado para acceder a un beneficio o programa de bienestar
/// </summary>
public class SolicitudBienestar : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid AsociadoId { get; set; }
    public Guid OrganizationId { get; set; }
    public Guid? ProgramaBienestarId { get; set; }

    /// <summary>Tipo de auxilio o beneficio solicitado</summary>
    public TipoAuxilio TipoAuxilio { get; set; }

    /// <summary>Monto solicitado</summary>
    public decimal MontoSolicitado { get; set; }

    /// <summary>Monto aprobado (si aplica)</summary>
    public decimal? MontoAprobado { get; set; }

    /// <summary>Estado actual de la solicitud</summary>
    public EstadoSolicitudBienestar Estado { get; set; } = EstadoSolicitudBienestar.Pendiente;

    /// <summary>Motivo o justificación de la solicitud</summary>
    public string Motivo { get; set; } = null!;

    /// <summary>Fecha en que se requiere el beneficio</summary>
    public DateTime FechaRequerida { get; set; }

    /// <summary>Fecha de aprobación o rechazo</summary>
    public DateTime? FechaResolucion { get; set; }

    /// <summary>Observaciones de quien resolvió</summary>
    public string? ObservacionesResolucion { get; set; }

    /// <summary>Id del usuario que resolvió la solicitud</summary>
    public Guid? ResueltoPorUserId { get; set; }

    // Navegación
    public User? Asociado { get; set; }
    public User? ResueltoPor { get; set; }
    public ProgramaBienestar? Programa { get; set; }
}
