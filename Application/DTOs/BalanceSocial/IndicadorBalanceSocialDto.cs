namespace Application.DTOs.BalanceSocial;

/// <summary>
/// DTO de indicador de balance social por asociado
/// </summary>
public record IndicadorBalanceSocialDto
{
    public Guid Id { get; init; }
    public Guid AsociadoId { get; init; }
    public Guid OrganizationId { get; init; }
    public int Anio { get; init; }
    public int HorasEducacion { get; init; }
    public int ParticipacionAsambleas { get; init; }
    public int ParticipacionComites { get; init; }
    public decimal AportesSociales { get; init; }
    public decimal BeneficiosRecibidos { get; init; }
    public bool CumpleEducacion { get; init; }
    public decimal IndiceBalanceSocial { get; init; }
    public string? Observaciones { get; init; }
}
