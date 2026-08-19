namespace Application.DTOs.Bienestar;

/// <summary>
/// DTO del fondo de solidaridad (10% excedentes Ley 79 art.54)
/// </summary>
public record FondoSolidaridadDto
{
    public Guid Id { get; init; }
    public Guid OrganizationId { get; init; }
    public DateTime Periodo { get; init; }
    public decimal TotalExcedentes { get; init; }
    public decimal AporteFondo { get; init; }
    public decimal SaldoDisponible { get; init; }
    public decimal TotalDesembolsado { get; init; }
    public bool Vigente { get; init; }
    public string? Observaciones { get; init; }
    public DateTime CreatedAt { get; init; }
}
