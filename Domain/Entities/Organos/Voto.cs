namespace Domain.Entities.Organos;

using Domain.Common;
using Domain.Enums;

/// <summary>
/// Voto de un asociado en una asamblea o sesión de órgano
/// </summary>
public class Voto : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid AsambleaId { get; set; }
    public Guid AsociadoId { get; set; }
    public TipoVoto VotoEmitido { get; set; }
    public DateTime Fecha { get; set; }
    public string? Observaciones { get; set; }

    // Navigation
    public Asamblea? Asamblea { get; set; }
}
