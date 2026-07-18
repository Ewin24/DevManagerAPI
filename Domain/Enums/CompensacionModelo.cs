namespace Domain.Enums;

/// <summary>
/// Modelos de compensación para asociados (CTAs) en el sector solidario
/// </summary>
public enum CompensacionModelo : byte
{
    /// <summary>Días trabajados × tarifa diaria</summary>
    DiasPorTarifa = 1,

    /// <summary>Valor fijo mensual</summary>
    FijoMensual = 2,

    /// <summary>Por proyecto u obra</summary>
    PorProyecto = 3
}
