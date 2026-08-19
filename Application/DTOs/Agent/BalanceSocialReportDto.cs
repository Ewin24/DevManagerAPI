namespace Application.DTOs.Agent;

/// <summary>
/// Reporte de Balance Social preparado por el Asistente Cooperativo
/// </summary>
public record BalanceSocialReportDto
{
    public required Guid OrganizationId { get; init; }
    public required string OrganizationName { get; init; }
    public int Anio { get; init; }
    public DateTime GeneradoEn { get; init; } = DateTime.UtcNow;
    public List<DimensionSocialDto> Dimensiones { get; init; } = new();
    public string? ResumenEjecutivo { get; init; }
    public List<string> Fortalezas { get; init; } = new();
    public List<string> OportunidadesMejora { get; init; } = new();
    public string? Narrativa { get; init; }
}

/// <summary>
/// Dimensión del Balance Social (gobernanza, educación, ética, etc.)
/// </summary>
public record DimensionSocialDto
{
    public required string Nombre { get; init; }
    public string? Descripcion { get; init; }
    public decimal Cobertura { get; init; } // 0-100%
    public decimal Meta { get; init; }
    public List<IndicadorSocialDto> Indicadores { get; init; } = new();
    public string? Estado => Cobertura switch
    {
        >= 80 => "Cumple",
        >= 50 => "En Progreso",
        _ => "No Cumple"
    };
}

/// <summary>
/// Indicador individual dentro de una dimensión social
/// </summary>
public record IndicadorSocialDto
{
    public required string Nombre { get; init; }
    public decimal ValorActual { get; init; }
    public decimal ValorMeta { get; init; }
    public string? Unidad { get; init; }
    public decimal Cobertura => ValorMeta > 0
        ? Math.Round(ValorActual / ValorMeta * 100, 1)
        : 0;
}
