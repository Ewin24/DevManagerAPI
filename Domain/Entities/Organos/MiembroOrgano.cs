namespace Domain.Entities.Organos;

using Domain.Common;

/// <summary>
/// Asignación de un asociado a un órgano de administración/control
/// </summary>
public class MiembroOrgano : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid OrganoId { get; set; }
    public Guid AsociadoId { get; set; }
    public string Cargo { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public bool Activo { get; set; } = true;

    // Navigation
    public Organo? Organo { get; set; }
}
