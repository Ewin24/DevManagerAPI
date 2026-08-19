namespace Application.DTOs.Excedentes;

/// <summary>
/// DTO para distribución de excedentes (Ley 79 art. 54)
/// </summary>
public record ExcedenteDto
{
    public Guid Id { get; init; }
    public Guid OrganizationId { get; init; }
    public DateTime Periodo { get; init; }
    public decimal TotalExcedentes { get; init; }
    public decimal ReservaProteccionAportes { get; init; }
    public decimal FondoEducacion { get; init; }
    public decimal FondoSolidaridad { get; init; }
    public decimal? Revalorizacion { get; init; }
    public decimal? RetornoCooperativo { get; init; }
    public bool AprobadoPorAsamblea { get; init; }
    public string? Observaciones { get; init; }
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// DTO para calcular distribución de excedentes
/// </summary>
public record CreateExcedenteDto
{
    public Guid OrganizationId { get; init; }
    public DateTime Periodo { get; init; }
    public decimal TotalExcedentes { get; init; }
    public string? Observaciones { get; init; }
}
