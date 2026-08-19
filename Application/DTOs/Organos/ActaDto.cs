namespace Application.DTOs.Organos;

public class ActaDto
{
    public Guid Id { get; set; }
    public Guid OrganoId { get; set; }
    public Guid? AsambleaId { get; set; }
    public DateTime Fecha { get; set; }
    public string TipoSesion { get; set; } = string.Empty;
    public int Quorum { get; set; }
    public string Decisiones { get; set; } = string.Empty;
    public string? ConvocatoriaUrl { get; set; }
    public string? ActaUrl { get; set; }
    public string? Observaciones { get; set; }
}

public class CreateActaDto
{
    public Guid OrganoId { get; set; }
    public Guid? AsambleaId { get; set; }
    public DateTime Fecha { get; set; }
    public string TipoSesion { get; set; } = "Ordinaria";
    public int Quorum { get; set; }
    public string Decisiones { get; set; } = string.Empty;
    public string? ConvocatoriaUrl { get; set; }
    public string? ActaUrl { get; set; }
    public string? Observaciones { get; set; }
}
