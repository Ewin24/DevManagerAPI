namespace Application.Services;

using System.Security.Cryptography;
using System.Text;
using Application.Common.Exceptions;
using Application.DTOs.Auth;
using Application.Interfaces;
using Domain.Entities.IAM;
using Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

/// <summary>
/// Implementación del servicio de autenticación
/// </summary>
public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IAuthRepository authRepository,
        IUserRepository userRepository,
        ITokenService tokenService,
        ILogger<AuthService> logger)
    {
        _authRepository = authRepository;
        _userRepository = userRepository;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        _logger.LogInformation("Intento de login para email: {Email}", request.Email);

        // Nota: En un flujo real, necesitaríamos obtener OrganizationId del email o subdomain
        // Por ahora, usamos un OrganizationId fijo para desarrollo
        var tempOrgId = Guid.Parse("B9E1BA94-9917-41B8-886E-117353BF3DE7"); // TODO: Implementar lógica de resolución de organización

        var user = await _authRepository.GetUserByEmailAsync(request.Email, tempOrgId);

        if (user == null || !user.IsActive)
        {
            _logger.LogWarning("Usuario no encontrado o inactivo: {Email}", request.Email);
            throw new UnauthorizedException("Credenciales inválidas");
        }

        // Verificar contraseña
        if (!VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt))
        {
            _logger.LogWarning("Contraseña incorrecta para: {Email}", request.Email);
            throw new UnauthorizedException("Credenciales inválidas");
        }

        // Actualizar último login
        await _userRepository.UpdateLastLoginAsync(user.Id);

        // Generar token
        var token = _tokenService.GenerateToken(user);

        return new LoginResponse
        {
            UserId = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            OrganizationId = user.OrganizationId,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddHours(8) // Configurable
        };
    }

    public async Task<RegisterOrganizationResponse> RegisterOrganizationAsync(
        RegisterOrganizationRequest request)
    {
        _logger.LogInformation("Registrando nueva organización: {OrgName}", request.OrganizationName);

        // Validar NIT único si se proporciona
        // TODO: Agregar validación de NIT único

        // Crear entidades
        var orgId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var (passwordHash, passwordSalt) = HashPassword(request.Password);

        var organization = new Organization
        {
            Id = orgId,
            Name = request.OrganizationName,
            LegalName = request.LegalName,
            Nit = request.Nit,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var adminUser = new User
        {
            Id = userId,
            OrganizationId = orgId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        // Registrar (transacción en SP)
        var (createdOrgId, createdUserId) = await _authRepository.RegisterOrganizationAsync(
            organization, adminUser);

        return new RegisterOrganizationResponse
        {
            OrganizationId = createdOrgId,
            AdminUserId = createdUserId
        };
    }

    #region Password Hashing Helpers

    private static (byte[] Hash, byte[] Salt) HashPassword(string password)
    {
        using var hmac = new HMACSHA512();
        var salt = hmac.Key;
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return (hash, salt);
    }

    private static bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt)
    {
        using var hmac = new HMACSHA512(storedSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return computedHash.SequenceEqual(storedHash);
    }

    #endregion
}
