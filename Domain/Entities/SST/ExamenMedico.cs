namespace Domain.Entities.SST;

using Domain.Common;
using Domain.Entities.IAM;
using Domain.Enums;

/// <summary>
/// Examen médico ocupacional según Res. 0312/2019
/// Tipos: Ingreso (preocupacional), Periódico, Retiro
/// </summary>
public class ExamenMedico : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid AsociadoId { get; set; }
    public Guid OrganizationId { get; set; }

    /// <summary>Tipo de examen: Ingreso, Periódico, Retiro</summary>
    public TipoExamenMedico TipoExamen { get; set; }

    /// <summary>Fecha programada para el examen</summary>
    public DateTime FechaProgramado { get; set; }

    /// <summary>Fecha en que se realizó efectivamente</summary>
    public DateTime? FechaRealizado { get; set; }

    /// <summary>Resultado del examen (Apto, Apto con restricción, No apto)</summary>
    public string? Resultado { get; set; }

    /// <summary>URL del archivo con el resultado</summary>
    public string? ArchivoUrl { get; set; }

    /// <summary>Observaciones del médico ocupacional</summary>
    public string? Observaciones { get; set; }

    // Navegación
    public User? Asociado { get; set; }
}
