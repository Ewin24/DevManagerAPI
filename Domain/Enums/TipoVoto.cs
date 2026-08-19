namespace Domain.Enums;

/// <summary>
/// Opciones de voto para asambleas y sesiones de órganos
/// </summary>
public enum TipoVoto
{
    /// <summary>Voto aprobatorio</summary>
    Aprobado = 1,

    /// <summary>Voto de rechazo</summary>
    Rechazado = 2,

    /// <summary>Abstención</summary>
    Abstencion = 3,

    /// <summary>Voto en blanco</summary>
    Blanco = 4
}
