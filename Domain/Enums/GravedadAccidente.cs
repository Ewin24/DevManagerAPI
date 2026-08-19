namespace Domain.Enums;

/// <summary>
/// Gravedad de un accidente laboral según clasificación ARL
/// </summary>
public enum GravedadAccidente : byte
{
    /// <summary>Sin lesión o lesión menor (primeros auxilios)</summary>
    Leve = 1,

    /// <summary>Lesión con incapacidad temporal</summary>
    Grave = 2,

    /// <summary>Accidente mortal</summary>
    Mortal = 3
}
