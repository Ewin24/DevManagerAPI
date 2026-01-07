namespace Domain.Enums;

/// <summary>
/// Origen de una evaluación de habilidad
/// </summary>
public enum SkillEvaluationSource : byte
{
    /// <summary>
    /// Evaluación generada automáticamente al completar proyecto
    /// </summary>
    Project = 1,

    /// <summary>
    /// Evaluación manual por RRHH o supervisor
    /// </summary>
    Manual = 2,

    /// <summary>
    /// Evaluación generada por regla del sistema/agente
    /// </summary>
    SystemRule = 3
}
