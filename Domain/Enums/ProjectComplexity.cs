namespace Domain.Enums;

/// <summary>
/// Nivel de complejidad del proyecto para cálculo de experiencia
/// </summary>
public enum ProjectComplexity : byte
{
    /// <summary>
    /// Proyecto de baja complejidad
    /// </summary>
    Low = 1,

    /// <summary>
    /// Proyecto de complejidad media
    /// </summary>
    Medium = 2,

    /// <summary>
    /// Proyecto de alta complejidad
    /// </summary>
    High = 3
}
