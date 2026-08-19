namespace Domain.Entities.Bienestar;

using Domain.Common;
using Domain.Entities.IAM;

/// <summary>
/// Programa de bienestar social ofrecido por la organización solidaria
/// </summary>
public class ProgramaBienestar : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }

    /// <summary>Nombre del programa de bienestar</summary>
    public string Nombre { get; set; } = null!;

    /// <summary>Descripción del programa</summary>
    public string? Descripcion { get; set; }

    /// <summary>Presupuesto asignado al programa</summary>
    public decimal Presupuesto { get; set; }

    /// <summary>Fecha de inicio del programa</summary>
    public DateTime FechaInicio { get; set; }

    /// <summary>Fecha de finalización del programa</summary>
    public DateTime? FechaFin { get; set; }

    /// <summary>¿Está activo el programa?</summary>
    public bool Activo { get; set; } = true;

    /// <summary>Máximo de beneficiarios permitidos</summary>
    public int? MaxBeneficiarios { get; set; }

    // Navegación
    public Organization? Organization { get; set; }
    public ICollection<SolicitudBienestar> Solicitudes { get; set; } = new List<SolicitudBienestar>();
}
