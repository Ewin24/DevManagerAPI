namespace Application.DTOs.SST;

/// <summary>
/// DTO para la matriz de riesgos laborales
/// </summary>
public record RiesgoDto
{
    public Guid Id { get; init; }
    public Guid OrganizationId { get; init; }
    public int NivelRiesgo { get; init; }
    public string NivelRiesgoNombre { get; init; } = null!;
    public string Factor { get; init; } = null!;
    public string Descripcion { get; init; } = null!;
    public bool Activo { get; init; }
    public string? Controles { get; init; }
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// DTO para crear un riesgo en la matriz
/// </summary>
public record CreateRiesgoDto
{
    public Guid OrganizationId { get; init; }
    public int NivelRiesgo { get; init; }
    public string Factor { get; init; } = null!;
    public string Descripcion { get; init; } = null!;
    public string? Controles { get; init; }
}
