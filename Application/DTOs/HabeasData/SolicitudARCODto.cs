namespace Application.DTOs.HabeasData;

using Domain.Enums;

/// <summary>
/// DTO para solicitud ARCO (Acceso, Rectificación, Cancelación, Oposición)
/// </summary>
public record SolicitudARCODto
{
    public Guid Id { get; init; }
    public Guid AsociadoId { get; init; }
    public Guid OrganizationId { get; init; }
    public TipoSolicitudARCO Tipo { get; init; }
    public string TipoNombre { get; init; } = null!;
    public DateTime Fecha { get; init; }
    public EstadoSolicitudARCO Estado { get; init; }
    public string EstadoNombre { get; init; } = null!;
    public string Descripcion { get; init; } = null!;
    public string? Respuesta { get; init; }
    public DateTime? FechaRespuesta { get; init; }
    public string? Radicado { get; init; }
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// DTO para crear una solicitud ARCO
/// </summary>
public record CreateSolicitudARCODto
{
    public Guid AsociadoId { get; init; }
    public Guid OrganizationId { get; init; }
    public TipoSolicitudARCO Tipo { get; init; }
    public string Descripcion { get; init; } = null!;
}
