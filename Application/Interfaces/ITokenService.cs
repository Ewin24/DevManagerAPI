namespace Application.Interfaces;

using Domain.Entities.IAM;

/// <summary>
/// Servicio para generación y validación de tokens JWT
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Genera un token JWT para el usuario autenticado
    /// </summary>
    string GenerateToken(User user);
}
