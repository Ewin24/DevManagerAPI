namespace Domain.Entities.Bienestar;

using Domain.Common;
using Domain.Entities.IAM;
using Domain.Enums;

/// <summary>
/// Auxilio individual entregado a un asociado desde el fondo de bienestar
/// </summary>
public class Auxilio : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid AsociadoId { get; set; }
    public Guid OrganizationId { get; set; }
    public Guid? SolicitudBienestarId { get; set; }
    public Guid? FondoSolidaridadId { get; set; }

    /// <summary>Tipo de auxilio entregado</summary>
    public TipoAuxilio Tipo { get; set; }

    /// <summary>Monto del auxilio entregado</summary>
    public decimal Monto { get; set; }

    /// <summary>Fecha de entrega o desembolso</summary>
    public DateTime FechaEntrega { get; set; }

    /// <summary>Descripción o concepto del auxilio</summary>
    public string Concepto { get; set; } = null!;

    /// <summary>¿Requiere reintegro?</summary>
    public bool RequiereReintegro { get; set; }

    /// <summary>Fecha límite de reintegro (si aplica)</summary>
    public DateTime? FechaLimiteReintegro { get; set; }

    // Navegación
    public User? Asociado { get; set; }
    public SolicitudBienestar? Solicitud { get; set; }
    public FondoSolidaridad? Fondo { get; set; }
}
