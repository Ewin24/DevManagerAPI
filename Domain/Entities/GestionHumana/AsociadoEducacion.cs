namespace Domain.Entities.GestionHumana;

using Domain.Common;
using Domain.Entities.IAM;

/// <summary>
/// Registro de inscripción y progreso de un asociado en un programa educativo
/// </summary>
public class AsociadoEducacion : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid AsociadoId { get; set; }
    public Guid ProgramaEducacionId { get; set; }
    public Guid OrganizationId { get; set; }

    /// <summary>Horas cursadas hasta el momento</summary>
    public int HorasCursadas { get; set; }

    /// <summary>Porcentaje de completitud (0-100)</summary>
    public decimal Progreso { get; set; }

    /// <summary>Fecha de inscripción</summary>
    public DateTime FechaInscripcion { get; set; }

    /// <summary>Fecha de finalización (null si no completado)</summary>
    public DateTime? FechaCompletado { get; set; }

    /// <summary>¿Completado satisfactoriamente?</summary>
    public bool Completado { get; set; }

    /// <summary>Calificación o resultado obtenido</summary>
    public string? Resultado { get; set; }

    // Navegación
    public User? Asociado { get; set; }
    public ProgramaEducacion? Programa { get; set; }
}
