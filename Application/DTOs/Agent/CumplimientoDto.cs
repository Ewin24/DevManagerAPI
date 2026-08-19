namespace Application.DTOs.Agent;

/// <summary>
/// Resultado de verificación de cumplimiento normativo cooperativo
/// </summary>
public record CumplimientoDto
{
    public required Guid OrganizationId { get; init; }
    public required string OrganizationName { get; init; }
    public DateTime VerificadoEn { get; init; } = DateTime.UtcNow;
    public List<AreaCumplimientoDto> Areas { get; init; } = new();
    public int AreasCumplen => Areas.Count(a => a.Cumple);
    public int AreasNoCumplen => Areas.Count(a => !a.Cumple);
    public decimal CoberturaGeneral => Areas.Count > 0
        ? Math.Round((decimal)AreasCumplen / Areas.Count * 100, 1)
        : 0;
    public string EstadoGeneral => CoberturaGeneral switch
    {
        >= 80 => "Cumple Satisfactoriamente",
        >= 50 => "Cumple Parcialmente",
        _ => "No Cumple"
    };
    public List<string> Alertas { get; init; } = new();
}

/// <summary>
/// Área específica de cumplimiento normativo
/// </summary>
public record AreaCumplimientoDto
{
    public required string Nombre { get; init; }
    public required string NormaAplicable { get; init; }
    public bool Cumple { get; init; }
    public decimal Cobertura { get; init; }
    public string? Detalle { get; init; }
    public List<string> Hallazgos { get; init; } = new();
}
