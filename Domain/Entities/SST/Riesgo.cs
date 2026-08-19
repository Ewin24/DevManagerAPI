namespace Domain.Entities.SST;

using Domain.Common;

/// <summary>
/// Matriz de riesgos laborales por organización solidaria
/// Identifica factores de riesgo y su nivel según SG-SST
/// </summary>
public class Riesgo : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }

    /// <summary>Nivel del riesgo (1-5: Bajo, Medio, Alto, Muy Alto, Crítico)</summary>
    public int NivelRiesgo { get; set; }

    /// <summary>Factor de riesgo (físico, químico, biomecánico, etc.)</summary>
    public string Factor { get; set; } = null!;

    /// <summary>Descripción del riesgo</summary>
    public string Descripcion { get; set; } = null!;

    /// <summary>¿Está activo en la matriz?</summary>
    public bool Activo { get; set; } = true;

    /// <summary>Controles implementados</summary>
    public string? Controles { get; set; }
}
