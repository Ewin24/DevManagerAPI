namespace Domain.Entities.GestionHumana;

using Domain.Common;
using Domain.Entities.IAM;

/// <summary>
/// Competencia cooperativa de un asociado para servicio solidario
/// (comités, asambleas, programas educativos)
/// </summary>
public class CompetenciaAsociado : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid AsociadoId { get; set; }
    public Guid OrganizationId { get; set; }

    /// <summary>Nombre de la competencia (ej. "Facilitación", "Contabilidad solidaria")</summary>
    public string Competencia { get; set; } = null!;

    /// <summary>Nivel de competencia (1-5)</summary>
    public int Nivel { get; set; }

    /// <summary>¿Está disponible para servicio solidario?</summary>
    public bool Disponible { get; set; } = true;

    /// <summary>Fecha de última actualización de la competencia</summary>
    public DateTime FechaActualizacion { get; set; }

    /// <summary>Observaciones sobre la competencia</summary>
    public string? Observaciones { get; set; }

    // Navegación
    public User? Asociado { get; set; }
}
