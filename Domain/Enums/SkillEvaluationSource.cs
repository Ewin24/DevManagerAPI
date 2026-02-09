namespace Domain.Enums;

/// <summary>
/// Origen de una evaluación de habilidad.
/// Valores alineados con config.EvaluationSources.
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
    SystemRule = 3,

    /// <summary>
    /// Evaluación derivada de una certificación verificada
    /// </summary>
    Certification = 4,

    /// <summary>
    /// Autoevaluación declarada por el empleado (requiere validación)
    /// </summary>
    SelfAssessment = 5
}