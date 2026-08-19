namespace Application.DTOs.Organos;

using Domain.Enums;

public class VotoDto
{
    public Guid Id { get; set; }
    public Guid AsambleaId { get; set; }
    public Guid AsociadoId { get; set; }
    public TipoVoto VotoEmitido { get; set; }
    public string VotoNombre => VotoEmitido.ToString();
    public DateTime Fecha { get; set; }
    public string? Observaciones { get; set; }
}

public class EmitirVotoDto
{
    public Guid AsambleaId { get; set; }
    public Guid AsociadoId { get; set; }
    public TipoVoto VotoEmitido { get; set; }
    public string? Observaciones { get; set; }
}

public class ResultadoVotacionDto
{
    public Guid AsambleaId { get; set; }
    public int TotalVotos { get; set; }
    public int Aprobados { get; set; }
    public int Rechazados { get; set; }
    public int Abstenciones { get; set; }
    public int Blancos { get; set; }
}
