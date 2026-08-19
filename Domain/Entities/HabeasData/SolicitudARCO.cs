namespace Domain.Entities.HabeasData;

using Domain.Common;
using Domain.Entities.IAM;
using Domain.Enums;

/// <summary>
/// Solicitud ARCO (Acceso, Rectificación, Cancelación, Oposición)
/// Derechos del titular de datos personales según Ley 1581/2012
/// </summary>
public class SolicitudARCO : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid AsociadoId { get; set; }
    public Guid OrganizationId { get; set; }

    /// <summary>Tipo de derecho ARCO ejercido</summary>
    public TipoSolicitudARCO Tipo { get; set; }

    /// <summary>Fecha de la solicitud</summary>
    public DateTime Fecha { get; set; }

    /// <summary>Estado actual de la solicitud</summary>
    public EstadoSolicitudARCO Estado { get; set; } = EstadoSolicitudARCO.Pendiente;

    /// <summary>Descripción detallada de la solicitud</summary>
    public string Descripcion { get; set; } = null!;

    /// <summary>Respuesta de la organización a la solicitud</summary>
    public string? Respuesta { get; set; }

    /// <summary>Fecha en que se dio respuesta</summary>
    public DateTime? FechaRespuesta { get; set; }

    /// <summary>Número de radicado de la solicitud</summary>
    public string? Radicado { get; set; }

    // Navegación
    public User? Asociado { get; set; }
}
