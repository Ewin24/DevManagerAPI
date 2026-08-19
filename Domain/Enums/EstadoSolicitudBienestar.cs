namespace Domain.Enums;

/// <summary>
/// Estados de una solicitud de bienestar para asociados
/// </summary>
public enum EstadoSolicitudBienestar : byte
{
    /// <summary>Solicitud ingresada, pendiente de revisión</summary>
    Pendiente = 1,

    /// <summary>En proceso de evaluación</summary>
    EnEvaluacion = 2,

    /// <summary>Aprobada</summary>
    Aprobada = 3,

    /// <summary>Rechazada</summary>
    Rechazada = 4,

    /// <summary>Beneficio entregado / desembolsado</summary>
    Entregada = 5
}
