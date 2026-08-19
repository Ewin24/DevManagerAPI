namespace Domain.Entities.Organos;

using Domain.Common;
using Domain.Enums;

/// <summary>
/// Órgano de administración, control o comité — Ley 79 art.26-45
/// </summary>
public class Organo : AuditableEntity
{
    public Guid Id { get; set; }
    public TipoOrgano Tipo { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public Guid OrganizationId { get; set; }
    public DateTime FechaConstitucion { get; set; }
    public string? Descripcion { get; set; }
    public bool Activo { get; set; } = true;

    // Navigation
    public ICollection<MiembroOrgano> Miembros { get; set; } = new List<MiembroOrgano>();
    public ICollection<Acta> Actas { get; set; } = new List<Acta>();
    public ICollection<Asamblea> Asambleas { get; set; } = new List<Asamblea>();
}
