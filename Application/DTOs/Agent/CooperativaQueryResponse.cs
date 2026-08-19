namespace Application.DTOs.Agent;

/// <summary>
/// Respuesta del Asistente Cooperativo a una consulta general
/// </summary>
public record CooperativaQueryResponse
{
    public required string Respuesta { get; init; }
    public string? Markdown { get; init; }
    public List<CitacionNormativa> Citations { get; init; } = new();
    public List<string> AccionesSugeridas { get; init; } = new();
    public Guid? ActionId { get; init; }
    public bool RequiereAprobacion { get; init; }
}

/// <summary>
/// Citación a una norma legal cooperativa
/// </summary>
public record CitacionNormativa
{
    public required string Norma { get; init; }
    public required string Articulo { get; init; }
    public string? Descripcion { get; init; }
    public string? UrlReferencia { get; init; }
}
