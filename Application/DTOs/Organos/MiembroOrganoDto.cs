namespace Application.DTOs.Organos;

public class MiembroOrganoDto
{
    public Guid Id { get; set; }
    public Guid OrganoId { get; set; }
    public Guid AsociadoId { get; set; }
    public string Cargo { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public bool Activo { get; set; }
}

public class AsignarMiembroDto
{
    public Guid OrganoId { get; set; }
    public Guid AsociadoId { get; set; }
    public string Cargo { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
}

public class UpdateMiembroDto
{
    public string? Cargo { get; set; }
    public DateTime? FechaFin { get; set; }
    public bool? Activo { get; set; }
}
