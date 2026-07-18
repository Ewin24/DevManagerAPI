namespace Domain.Enums;

/// <summary>
/// Tipo de organización del sector solidario colombiano
/// </summary>
public enum TipoOrganizacionSolidaria : byte
{
    /// <summary>Cooperativa (Ley 79/1988)</summary>
    Cooperativa = 1,

    /// <summary>Fondo de Empleados (Decreto-Ley 1481/1989)</summary>
    FondoEmpleados = 2,

    /// <summary>Mutual (Ley 24/1981 y Decreto 1480/1989)</summary>
    Mutual = 3,

    /// <summary>Asociación mutual</summary>
    Asociacion = 4,

    /// <summary>Federación de cooperativas</summary>
    Federacion = 5,

    /// <summary>Confederación de cooperativas</summary>
    Confederacion = 6
}
