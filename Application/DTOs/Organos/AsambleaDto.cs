namespace Application.DTOs.Organos;

using Domain.Enums;

public class AsambleaDto
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public Guid? OrganoId { get; set; }
    public DateTime Fecha { get; set; }
    public TipoAsamblea Tipo { get; set; }
    public string TipoNombre => Tipo.ToString();
    public string Convocatoria { get; set; } = string.Empty;
    public int QuorumMinimo { get; set; }
    public int? Asistentes { get; set; }
    public bool Cerrada { get; set; }
    public DateTime? FechaCierre { get; set; }
    public string? Resultados { get; set; }
    public int VotosCount { get; set; }
}

public class ConvocarAsambleaDto
{
    public Guid OrganizationId { get; set; }
    public Guid? OrganoId { get; set; }
    public DateTime Fecha { get; set; }
    public TipoAsamblea Tipo { get; set; }
    public string Convocatoria { get; set; } = string.Empty;
    public int QuorumMinimo { get; set; }
}

public class RegistrarAsistenciaDto
{
    public int Asistentes { get; set; }
}

public class CerrarAsambleaDto
{
    public string? Resultados { get; set; }
}
