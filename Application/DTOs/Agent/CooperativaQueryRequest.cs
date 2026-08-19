namespace Application.DTOs.Agent;

/// <summary>
/// Solicitud de consulta al Asistente Cooperativo
/// </summary>
public record CooperativaQueryRequest
{
    public required string Consulta { get; init; }
    public string? Contexto { get; init; }
    public bool RequerirAprobacion { get; init; }
}

/// <summary>
/// Solicitud para generar un reporte de Balance Social
/// </summary>
public record GenerarBalanceSocialRequest
{
    public required Guid OrganizationId { get; init; }
    public int Anio { get; init; } = DateTime.UtcNow.Year;
    public bool IncluirRecomendaciones { get; init; } = true;
}

/// <summary>
/// Solicitud para verificar cumplimiento normativo cooperativo
/// </summary>
public record VerificarCumplimientoRequest
{
    public required Guid OrganizationId { get; init; }
    public bool VerificarEducacion { get; init; } = true;
    public bool VerificarSST { get; init; } = true;
    public bool VerificarHabeasData { get; init; } = true;
    public bool VerificarAportes { get; init; } = true;
}

/// <summary>
/// Solicitud para responder una duda de asociado
/// </summary>
public record ResponderDudaRequest
{
    public required Guid OrganizationId { get; init; }
    public required string Pregunta { get; init; }
    public string? TipoAsociado { get; init; }
}
