namespace Domain.Enums;

/// <summary>
/// Tipo de persona en el contexto solidario
/// </summary>
public enum PersonType : byte
{
    /// <summary>Asociado (miembro de la cooperativa/fondo/mutual)</summary>
    Asociado = 1,

    /// <summary>Empleado (trabajador de la organización)</summary>
    Empleado = 2,

    /// <summary>Ambos — persona que es asociado y empleado simultáneamente</summary>
    Both = 3
}
