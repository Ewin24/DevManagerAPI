namespace Domain.Enums;

/// <summary>
/// Estado de una postulación a proyecto
/// </summary>
public enum ApplicationStatus : byte
{
    /// <summary>
    /// Postulación enviada, pendiente de revisión
    /// </summary>
    Applied = 1,

    /// <summary>
    /// Postulación aprobada
    /// </summary>
    Approved = 2,

    /// <summary>
    /// Postulación rechazada
    /// </summary>
    Rejected = 3,

    /// <summary>
    /// Postulación retirada por el usuario
    /// </summary>
    Withdrawn = 4
}
