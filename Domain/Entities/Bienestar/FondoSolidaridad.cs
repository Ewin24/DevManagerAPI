namespace Domain.Entities.Bienestar;

using Domain.Common;

/// <summary>
/// Fondo de solidaridad: representa el 10% de excedentes según Ley 79 art.54
/// </summary>
public class FondoSolidaridad : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }

    /// <summary>Período del fondo (mes/año)</summary>
    public DateTime Periodo { get; set; }

    /// <summary>Total de excedentes del período</summary>
    public decimal TotalExcedentes { get; set; }

    /// <summary>10% destinado al fondo (Ley 79 art.54)</summary>
    public decimal AporteFondo { get; set; }

    /// <summary>Saldo disponible del fondo</summary>
    public decimal SaldoDisponible { get; set; }

    /// <summary>Total desembolsado en auxilios</summary>
    public decimal TotalDesembolsado { get; set; }

    /// <summary>¿Está vigente este período del fondo?</summary>
    public bool Vigente { get; set; } = true;

    /// <summary>Observaciones sobre el cierre del período</summary>
    public string? Observaciones { get; set; }
}
