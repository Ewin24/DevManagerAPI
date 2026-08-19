namespace Domain.Enums;

/// <summary>
/// Tipos de solicitud ARCO (Acceso, Rectificación, Cancelación, Oposición)
/// según Ley 1581/2012
/// </summary>
public enum TipoSolicitudARCO : byte
{
    /// <summary>Solicitud de acceso a datos personales</summary>
    Acceso = 1,

    /// <summary>Solicitud de rectificación o corrección</summary>
    Rectificacion = 2,

    /// <summary>Solicitud de cancelación o supresión</summary>
    Cancelacion = 3,

    /// <summary>Solicitud de oposición al tratamiento</summary>
    Oposicion = 4
}
