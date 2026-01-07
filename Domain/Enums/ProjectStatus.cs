namespace Domain.Enums;

/// <summary>
/// Estados del ciclo de vida de un proyecto
/// </summary>
public enum ProjectStatus : byte
{
    /// <summary>
    /// Proyecto en borrador, no visible para postulaciones
    /// </summary>
    Draft = 1,

    /// <summary>
    /// Proyecto abierto para postulaciones
    /// </summary>
    Open = 2,

    /// <summary>
    /// Proyecto en progreso activo
    /// </summary>
    InProgress = 3,

    /// <summary>
    /// Proyecto completado exitosamente
    /// </summary>
    Closed = 4,

    /// <summary>
    /// Proyecto cancelado antes de completar
    /// </summary>
    Cancelled = 5
}
