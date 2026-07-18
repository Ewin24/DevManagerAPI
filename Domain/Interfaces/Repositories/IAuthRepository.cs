namespace Domain.Interfaces.Repositories;

using Domain.Entities.IAM;

/// <summary>
/// Repositorio para operaciones de autenticación
/// </summary>
public interface IAuthRepository
{
    /// <summary>
    /// Obtiene un usuario por email (para login)
    /// </summary>
    Task<User?> GetUserByEmailAsync(string email, Guid organizationId);

    /// <summary>
    /// Registra una nueva organización con su usuario administrador (transacción)
    /// </summary>
    Task<(Guid OrganizationId, Guid UserId)> RegisterOrganizationAsync(
        Organization organization,
        User adminUser);
}
