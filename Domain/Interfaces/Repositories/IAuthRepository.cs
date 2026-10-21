namespace Domain.Interfaces.Repositories;

using Domain.Entities.IAM;

/// <summary>
/// Repositorio para operaciones de autenticación
/// </summary>
public interface IAuthRepository
{
    /// <summary>
    /// Obtiene los usuarios (no eliminados) con ese email en todas las organizaciones (para login)
    /// </summary>
    Task<IEnumerable<User>> GetUsersByEmailAsync(string email);

    /// <summary>
    /// Verifica si ya existe una organización con el NIT indicado
    /// </summary>
    Task<bool> NitExistsAsync(string nit);

    /// <summary>
    /// Registra una nueva organización con su usuario administrador (transacción)
    /// </summary>
    Task<(Guid OrganizationId, Guid UserId)> RegisterOrganizationAsync(
        Organization organization,
        User adminUser);
}
