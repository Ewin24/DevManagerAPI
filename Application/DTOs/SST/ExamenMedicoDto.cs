namespace Application.DTOs.SST;

using Domain.Enums;

/// <summary>
/// DTO para examen médico ocupacional
/// </summary>
public record ExamenMedicoDto
{
    public Guid Id { get; init; }
    public Guid AsociadoId { get; init; }
    public Guid OrganizationId { get; init; }
    public TipoExamenMedico TipoExamen { get; init; }
    public string TipoExamenNombre { get; init; } = null!;
    public DateTime FechaProgramado { get; init; }
    public DateTime? FechaRealizado { get; init; }
    public string? Resultado { get; init; }
    public string? ArchivoUrl { get; init; }
    public string? Observaciones { get; init; }
    public bool Realizado => FechaRealizado.HasValue;
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// DTO para crear un examen médico ocupacional
/// </summary>
public record CreateExamenMedicoDto
{
    public Guid AsociadoId { get; init; }
    public Guid OrganizationId { get; init; }
    public TipoExamenMedico TipoExamen { get; init; }
    public DateTime FechaProgramado { get; init; }
}
