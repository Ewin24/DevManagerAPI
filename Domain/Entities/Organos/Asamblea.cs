namespace Domain.Entities.Organos;

using Domain.Common;
using Domain.Enums;

/// <summary>
/// Asamblea General de Asociados — máximo órgano de administración (Ley 79 art.26-33)
/// </summary>
public class Asamblea : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Guid? OrganoId { get; set; }
    public DateTime Fecha { get; set; }
    public TipoAsamblea Tipo { get; set; }
    public string Convocatoria { get; set; } = string.Empty;
    public int QuorumMinimo { get; set; }
    public int? Asistentes { get; set; }
    public bool Cerrada { get; set; }
    public DateTime? FechaCierre { get; set; }
    public string? Resultados { get; set; }

    // Navigation
    public Organo? Organo { get; set; }
    public ICollection<Voto> Votos { get; set; } = new List<Voto>();
    public ICollection<Acta> Actas { get; set; } = new List<Acta>();
}
