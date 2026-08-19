namespace Application.DTOs.Organos;

using Domain.Enums;

public class OrganoDto
{
    public Guid Id { get; set; }
    public TipoOrgano Tipo { get; set; }
    public string TipoNombre => Tipo.ToString();
    public string Nombre { get; set; } = string.Empty;
    public Guid OrganizationId { get; set; }
    public DateTime FechaConstitucion { get; set; }
    public string? Descripcion { get; set; }
    public bool Activo { get; set; }
    public int MiembrosCount { get; set; }
    public int ActasCount { get; set; }
}

public class CreateOrganoDto
{
    public TipoOrgano Tipo { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public Guid OrganizationId { get; set; }
    public DateTime FechaConstitucion { get; set; }
    public string? Descripcion { get; set; }
}

public class UpdateOrganoDto
{
    public string? Nombre { get; set; }
    public string? Descripcion { get; set; }
    public bool? Activo { get; set; }
}
