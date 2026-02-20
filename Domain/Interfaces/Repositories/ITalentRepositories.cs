namespace Domain.Interfaces.Repositories;

using Domain.Entities.Talent;

/// <summary>
/// Repositorio para perfiles de empleados
/// </summary>
public interface IProfileRepository
{
    /// <summary>
    /// Obtiene el perfil de un empleado por UserId
    /// </summary>
    Task<EmployeeProfile?> GetByUserIdAsync(Guid userId, Guid organizationId);

    /// <summary>
    /// Obtiene todos los perfiles de una organización con sus skills
    /// </summary>
    Task<IEnumerable<EmployeeProfile>> GetAllAsync(Guid organizationId);

    /// <summary>
    /// Crea o actualiza el perfil de un empleado
    /// </summary>
    Task<bool> UpsertAsync(EmployeeProfile profile);

    /// <summary>
    /// Elimina lógicamente el perfil de un empleado
    /// </summary>
    Task<bool> SoftDeleteAsync(Guid userId, Guid organizationId, Guid deletedByUserId);
}

/// <summary>
/// Repositorio para catálogo de habilidades
/// </summary>
public interface ISkillRepository
{
    /// <summary>
    /// Obtiene todas las habilidades (globales + de la organización)
    /// </summary>
    Task<IEnumerable<Skill>> GetAllAsync(Guid organizationId);

    /// <summary>
    /// Obtiene una habilidad por ID
    /// </summary>
    Task<Skill?> GetByIdAsync(Guid id);

    /// <summary>
    /// Crea una nueva habilidad
    /// </summary>
    Task<Guid> CreateAsync(Skill skill);

    /// <summary>
    /// Actualiza una habilidad existente
    /// </summary>
    Task<bool> UpdateAsync(Skill skill);

    /// <summary>
    /// Elimina lógicamente una habilidad
    /// </summary>
    Task<bool> SoftDeleteAsync(Guid id, Guid organizationId, Guid deletedByUserId);

    /// <summary>
    /// Verifica si una habilidad con el mismo nombre ya existe
    /// </summary>
    Task<bool> ExistsByNameAsync(string name, Guid? organizationId);
}

/// <summary>
/// Repositorio para habilidades de empleados
/// </summary>
public interface IEmployeeSkillRepository
{
    /// <summary>
    /// Obtiene todas las habilidades de un empleado
    /// </summary>
    Task<IEnumerable<EmployeeSkill>> GetByUserIdAsync(Guid userId, Guid organizationId);

    /// <summary>
    /// Crea o actualiza una habilidad de empleado
    /// </summary>
    Task<Guid> UpsertAsync(EmployeeSkill employeeSkill);

    /// <summary>
    /// Valida una habilidad (actualiza LastValidatedAt y ValidatorId)
    /// </summary>
    Task<bool> ValidateSkillAsync(Guid id, Guid organizationId, Guid? validatorId, byte? newLevel = null);

    /// <summary>
    /// Actualiza el nivel de una habilidad (usado por el agente)
    /// </summary>
    Task<bool> UpdateLevelAsync(Guid userId, Guid skillId, byte newLevel, Guid organizationId);

    /// <summary>
    /// Elimina lógicamente una habilidad de empleado
    /// </summary>
    Task<bool> SoftDeleteAsync(Guid id, Guid organizationId, Guid deletedByUserId);

}

/// <summary>
/// Repositorio para certificaciones
/// </summary>
public interface ICertificationRepository
{
    Task<IEnumerable<Certification>> GetCertificationsByUserIdAsync(Guid userId, Guid organizationId);
    Task<Certification?> GetCertificationByIdAsync(Guid id);
    Task<Guid> CreateCertificationAsync(Certification certification);
    Task<bool> UpdateCertificationAsync(Certification certification);
    Task<bool> SoftDeleteCertificationAsync(Guid id, Guid organizationId, Guid deletedByUserId);
}

/// <summary>
/// Repositorio para evaluaciones de habilidades (historial)
/// </summary>
public interface ISkillEvaluationRepository
{
    /// <summary>
    /// Registra una nueva evaluación de habilidad
    /// </summary>
    Task<Guid> CreateAsync(SkillEvaluation evaluation);

    /// <summary>
    /// Obtiene el historial de evaluaciones de un usuario
    /// </summary>
    Task<IEnumerable<SkillEvaluation>> GetByUserIdAsync(Guid userId, Guid organizationId);
}