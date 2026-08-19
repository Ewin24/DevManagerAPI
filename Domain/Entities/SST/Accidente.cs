namespace Domain.Entities.SST;

using Domain.Common;
using Domain.Entities.IAM;
using Domain.Enums;

/// <summary>
/// Accidente de trabajo reportado según Decreto 2150/2017 y FURAT
/// Incluye investigación obligatoria en 15 días hábiles
/// </summary>
public class Accidente : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid AsociadoId { get; set; }
    public Guid OrganizationId { get; set; }

    /// <summary>Fecha del accidente</summary>
    public DateTime Fecha { get; set; }

    /// <summary>Tipo de accidente (caída, golpe, tránsito, etc.)</summary>
    public string Tipo { get; set; } = null!;

    /// <summary>Gravedad del accidente</summary>
    public GravedadAccidente Gravedad { get; set; }

    /// <summary>Nombre o código de la ARL</summary>
    public string ARL { get; set; } = null!;

    /// <summary>Descripción detallada del accidente</summary>
    public string Descripcion { get; set; } = null!;

    /// <summary>Número de FURAT (Formato Único de Reporte de Accidentes)</summary>
    public string? FURAT { get; set; }

    /// <summary>Fecha de investigación del accidente (máx. 15 días hábiles)</summary>
    public DateTime? FechaInvestigacion { get; set; }

    /// <summary>¿Se completó la investigación en 15 días?</summary>
    public bool InvestigacionCompletada { get; set; }

    /// <summary>Conclusiones de la investigación</summary>
    public string? Conclusiones { get; set; }

    /// <summary>Causas identificadas</summary>
    public string? Causas { get; set; }

    /// <summary>Medidas correctivas tomadas</summary>
    public string? MedidasCorrectivas { get; set; }

    // Navegación
    public User? Asociado { get; set; }
}
