namespace Application.DTOs.Bienestar;

using Domain.Enums;

/// <summary>
/// DTO de creación de solicitud de bienestar
/// </summary>
public record CreateSolicitudBienestarDto
{
    public Guid AsociadoId { get; init; }
    public Guid OrganizationId { get; init; }
    public Guid? ProgramaBienestarId { get; init; }
    public TipoAuxilio TipoAuxilio { get; init; }
    public decimal MontoSolicitado { get; init; }
    public string Motivo { get; init; } = null!;
    public DateTime FechaRequerida { get; init; }
}
