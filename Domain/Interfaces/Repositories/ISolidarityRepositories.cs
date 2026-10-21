namespace Domain.Interfaces.Repositories;

using Domain.Entities.Solidarity;
using Domain.Enums;

/// <summary>
/// Repositorio de asociados y sus periodos de membresía (todo filtrado por OrganizationId)
/// </summary>
public interface IAssociateRepository
{
    /// <summary>Lista asociados (con usuario y periodos) de una organización</summary>
    Task<IEnumerable<Associate>> GetAllAsync(Guid organizationId, AssociateStatus? status = null);

    /// <summary>Obtiene un asociado (con usuario y periodos) por el ID del usuario</summary>
    Task<Associate?> GetByUserIdAsync(Guid userId, Guid organizationId);

    /// <summary>Crea el asociado junto con su primer periodo</summary>
    Task CreateAsync(Associate associate);

    /// <summary>Persiste el estado del asociado y agrega/actualiza sus periodos en una sola operación</summary>
    Task UpdateAsync(Associate associate);
}

/// <summary>
/// Repositorio de políticas de datos y consentimientos (todo filtrado por OrganizationId)
/// </summary>
public interface IConsentRepository
{
    Task<IEnumerable<DataPolicyVersion>> GetPoliciesAsync(Guid organizationId);
    Task<DataPolicyVersion?> GetPolicyByIdAsync(Guid id, Guid organizationId);
    Task<DataPolicyVersion?> GetCurrentPolicyAsync(Guid organizationId);
    Task<bool> PolicyVersionExistsAsync(Guid organizationId, string version);
    Task CreatePolicyAsync(DataPolicyVersion policy);

    Task<IEnumerable<DataConsent>> GetConsentsAsync(Guid userId, Guid organizationId);
    Task<DataConsent?> GetConsentByIdAsync(Guid id, Guid organizationId);

    /// <summary>True si el usuario tiene al menos un consentimiento sin revocar</summary>
    Task<bool> HasActiveConsentAsync(Guid userId, Guid organizationId);

    /// <summary>True si el usuario ya tiene un consentimiento sin revocar para esa versión de política</summary>
    Task<bool> HasActiveConsentForPolicyAsync(Guid userId, Guid policyVersionId, Guid organizationId);

    Task CreateConsentAsync(DataConsent consent);
    Task UpdateConsentAsync(DataConsent consent);
}

/// <summary>
/// Repositorio de programas de educación cooperativa e inscripciones (todo filtrado por OrganizationId)
/// </summary>
public interface IEducationRepository
{
    Task<IEnumerable<EducationProgram>> GetProgramsAsync(Guid organizationId);
    Task<EducationProgram?> GetProgramByIdAsync(Guid id, Guid organizationId);
    Task CreateProgramAsync(EducationProgram program);
    Task UpdateProgramAsync(EducationProgram program);

    /// <summary>Borrado lógico del programa</summary>
    Task DeleteProgramAsync(Guid id, Guid organizationId, Guid deletedByUserId);

    /// <summary>Inscripciones de un programa (con nombre del asociado)</summary>
    Task<IEnumerable<EducationEnrollment>> GetEnrollmentsByProgramAsync(Guid programId, Guid organizationId);

    /// <summary>Todas las inscripciones de la organización en programas vigentes (con tipo de programa)</summary>
    Task<IEnumerable<EducationEnrollment>> GetEnrollmentsAsync(Guid organizationId);

    Task<EducationEnrollment?> GetEnrollmentByIdAsync(Guid id, Guid organizationId);
    Task<bool> EnrollmentExistsAsync(Guid programId, Guid userId, Guid organizationId);
    Task CreateEnrollmentAsync(EducationEnrollment enrollment);
    Task UpdateEnrollmentAsync(EducationEnrollment enrollment);

    /// <summary>Horas completadas de educación básica del asociado (todas las fechas)</summary>
    Task<decimal> GetCompletedBasicHoursAsync(Guid userId, Guid organizationId);
}
