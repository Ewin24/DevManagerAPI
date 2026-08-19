namespace Domain.Entities.BalanceSocial;

using Domain.Common;

/// <summary>
/// Indicador de balance social por asociado y período
/// Mide contribuciones solidarias, participación en gobierno,
/// horas de educación, y otros indicadores de gestión social
/// </summary>
public class IndicadorBalanceSocial : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid AsociadoId { get; set; }
    public Guid OrganizationId { get; set; }

    /// <summary>Período del indicador (año)</summary>
    public int Anio { get; set; }

    /// <summary>Horas de educación cooperativa completadas en el período</summary>
    public int HorasEducacion { get; set; }

    /// <summary>Participaciones en asambleas (número de asistencias)</summary>
    public int ParticipacionAsambleas { get; set; }

    /// <summary>Participación en comités y consejos</summary>
    public int ParticipacionComites { get; set; }

    /// <summary>Total de aportes sociales en el período</summary>
    public decimal AportesSociales { get; set; }

    /// <summary>Beneficios recibidos del fondo de bienestar</summary>
    public decimal BeneficiosRecibidos { get; set; }

    /// <summary>¿Cumple con el mínimo de horas de educación (20hr art.88-91)?</summary>
    public bool CumpleEducacion { get; set; }

    /// <summary>Índice compuesto de balance social (0-100)</summary>
    public decimal IndiceBalanceSocial { get; set; }

    /// <summary>Observaciones del período</summary>
    public string? Observaciones { get; set; }
}
