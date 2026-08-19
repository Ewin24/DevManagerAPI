namespace Domain.Entities.Organos;

using Domain.Common;
using Domain.Enums;

/// <summary>
/// Acta de sesión de un órgano de administración o control
/// </summary>
public class Acta : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid OrganoId { get; set; }
    public Guid? AsambleaId { get; set; }
    public DateTime Fecha { get; set; }
    public string TipoSesion { get; set; } = string.Empty; // Ordinaria, Extraordinaria
    public int Quorum { get; set; }
    public string Decisiones { get; set; } = string.Empty;
    public string? ConvocatoriaUrl { get; set; }
    public string? ActaUrl { get; set; }
    public string? Observaciones { get; set; }

    // Navigation
    public Organo? Organo { get; set; }
    public Asamblea? Asamblea { get; set; }
}
