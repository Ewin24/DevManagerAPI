namespace Application.DTOs.Bienestar;

using Domain.Enums;

/// <summary>
/// DTO de solicitud de bienestar de un asociado
/// </summary>
public record SolicitudBienestarDto
{
    public Guid Id { get; init; }
    public Guid AsociadoId { get; init; }
    public Guid OrganizationId { get; init; }
    public Guid? ProgramaBienestarId { get; init; }
    public string? ProgramaNombre { get; init; }
    public TipoAuxilio TipoAuxilio { get; init; }
    public string TipoAuxilioNombre { get; init; } = null!;
    public decimal MontoSolicitado { get; init; }
    public decimal? MontoAprobado { get; init; }
    public EstadoSolicitudBienestar Estado { get; init; }
    public string EstadoNombre { get; init; } = null!;
    public string Motivo { get; init; } = null!;
    public DateTime FechaRequerida { get; init; }
    public DateTime? FechaResolucion { get; init; }
    public string? ObservacionesResolucion { get; init; }
    public DateTime CreatedAt { get; init; }
}
