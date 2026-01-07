namespace Application.Interfaces;

using Application.DTOs.Auth;

/// <summary>
/// Servicio de autenticación con lógica de negocio
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Autentica un usuario y genera token JWT
    /// </summary>
    Task<LoginResponse> LoginAsync(LoginRequest request);

    /// <summary>
    /// Registra una nueva organización con usuario admin
    /// </summary>
    Task<RegisterOrganizationResponse> RegisterOrganizationAsync(RegisterOrganizationRequest request);
}
