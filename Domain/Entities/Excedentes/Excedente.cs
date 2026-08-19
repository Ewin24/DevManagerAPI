namespace Domain.Entities.Excedentes;

using Domain.Common;

/// <summary>
/// Distribución de excedentes según Ley 79 art. 54
/// 20% Reserva Protección Aportes, 20% Fondo Educación, 10% Fondo Solidaridad
/// El remanente se distribuye según lo determine la Asamblea General
/// </summary>
public class Excedente : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }

    /// <summary>Periodo contable (año/mes)</summary>
    public DateTime Periodo { get; set; }

    /// <summary>Total de excedentes del período</summary>
    public decimal TotalExcedentes { get; set; }

    /// <summary>20% - Reserva de Protección de Aportes</summary>
    public decimal ReservaProteccionAportes { get; set; }

    /// <summary>20% - Fondo de Educación</summary>
    public decimal FondoEducacion { get; set; }

    /// <summary>10% - Fondo de Solidaridad</summary>
    public decimal FondoSolidaridad { get; set; }

    /// <summary>Revalorización de aportes (remanente aprobado por Asamblea)</summary>
    public decimal? Revalorizacion { get; set; }

    /// <summary>Retorno cooperativo (remanente aprobado por Asamblea)</summary>
    public decimal? RetornoCooperativo { get; set; }

    /// <summary>¿Se ha aprobado la distribución por Asamblea?</summary>
    public bool AprobadoPorAsamblea { get; set; }

    /// <summary>Observaciones sobre la distribución</summary>
    public string? Observaciones { get; set; }
}
