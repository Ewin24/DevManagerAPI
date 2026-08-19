namespace Domain.Enums;

/// <summary>
/// Estados de una solicitud ARCO (Ley 1581/2012)
/// </summary>
public enum EstadoSolicitudARCO : byte
{
    /// <summary>Solicitud recibida, pendiente de atención</summary>
    Pendiente = 1,

    /// <summary>Solicitud atendida y resuelta</summary>
    Atendida = 2,

    /// <summary>Solicitud rechazada por causal legal</summary>
    Rechazada = 3
}
