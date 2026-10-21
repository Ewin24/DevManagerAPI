namespace Domain.Interfaces.Repositories;

using Domain.Entities.IAM;

/// <summary>
/// Repositorio para gestión de usuarios
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Obtiene todos los usuarios de una organización
    /// </summary>
    Task<IEnumerable<User>> GetAllAsync(Guid organizationId);

    /// <summary>
    /// Obtiene un usuario por ID
    /// </summary>
    Task<User?> GetByIdAsync(Guid id, Guid organizationId);

    /// <summary>
    /// Verifica si un email ya existe en la organización
    /// </summary>
    Task<bool> EmailExistsAsync(string email, Guid organizationId, Guid? excludeUserId = null);

    /// <summary>
    /// Verifica si un documento (tipo + número) ya existe en la organización
    /// </summary>
    Task<bool> DocumentExistsAsync(Domain.Enums.DocumentType documentType, string documentNumber, Guid organizationId, Guid? excludeUserId = null);

    /// <summary>
    /// Único escritor de los campos de documento del usuario
    /// </summary>
    Task<bool> SetDocumentAsync(Guid id, Guid organizationId, Domain.Enums.DocumentType documentType, string documentNumber, Guid updatedByUserId);

    /// <summary>
    /// Crea un nuevo usuario
    /// </summary>
    Task<Guid> CreateAsync(User user);

    /// <summary>
    /// Actualiza un usuario existente
    /// </summary>
    Task<bool> UpdateAsync(User user);

    /// <summary>
    /// Elimina lógicamente un usuario
    /// </summary>
    Task<bool> SoftDeleteAsync(Guid id, Guid organizationId, Guid deletedByUserId);

    /// <summary>
    /// Actualiza la fecha de último login
    /// </summary>
    Task UpdateLastLoginAsync(Guid userId);
}
