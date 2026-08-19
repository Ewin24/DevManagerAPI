namespace Application.DTOs.SST;

using Domain.Enums;

/// <summary>
/// DTO para accidente de trabajo (FURAT)
/// </summary>
public record AccidenteDto
{
    public Guid Id { get; init; }
    public Guid AsociadoId { get; init; }
    public Guid OrganizationId { get; init; }
    public DateTime Fecha { get; init; }
    public string Tipo { get; init; } = null!;
    public GravedadAccidente Gravedad { get; init; }
    public string GravedadNombre { get; init; } = null!;
    public string ARL { get; init; } = null!;
    public string Descripcion { get; init; } = null!;
    public string? FURAT { get; init; }
    public DateTime? FechaInvestigacion { get; init; }
    public bool InvestigacionCompletada { get; init; }
    public string? Conclusiones { get; init; }
    public string? Causas { get; init; }
    public string? MedidasCorrectivas { get; init; }
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// DTO para reportar un accidente de trabajo
/// </summary>
public record CreateAccidenteDto
{
    public Guid AsociadoId { get; init; }
    public Guid OrganizationId { get; init; }
    public DateTime Fecha { get; init; }
    public string Tipo { get; init; } = null!;
    public GravedadAccidente Gravedad { get; init; }
    public string ARL { get; init; } = null!;
    public string Descripcion { get; init; } = null!;
    public string? FURAT { get; init; }
}
