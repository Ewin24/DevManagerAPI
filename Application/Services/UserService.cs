namespace Application.Services;

using Application.Common.Exceptions;
using Application.DTOs.Users;
using Application.Interfaces;
using Domain.Entities.IAM;
using Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Implementación del servicio de gestión de usuarios
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IRolePermissionRepository _rolePermissionRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IUserRepository userRepository,
        IRolePermissionRepository rolePermissionRepository,
        ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _rolePermissionRepository = rolePermissionRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<UserResponse>> GetAllAsync(Guid organizationId)
    {
        var users = await _userRepository.GetAllAsync(organizationId);
        var userResponses = new List<UserResponse>();
        
        foreach (var user in users)
        {
            var response = await MapToResponseAsync(user, organizationId);
            userResponses.Add(response);
        }
        
        return userResponses;
    }

    public async Task<UserResponse> GetByIdAsync(Guid id, Guid organizationId)
    {
        var user = await _userRepository.GetByIdAsync(id, organizationId);
        if (user == null)
        {
            throw new NotFoundException("Usuario", id);
        }
        return await MapToResponseAsync(user, organizationId);
    }

    public async Task<UserResponse> CreateAsync(
        CreateUserRequest request,
        Guid organizationId,
        Guid createdByUserId)
    {
        // Validar email único
        if (await _userRepository.EmailExistsAsync(request.Email, organizationId))
        {
            throw new ConflictException($"El email '{request.Email}' ya está registrado");
        }

        var (passwordHash, passwordSalt) = HashPassword(request.Password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            OrganizationId = organizationId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = createdByUserId
        };

        await _userRepository.CreateAsync(user);

        _logger.LogInformation("Usuario creado: {UserId} - {Email}", user.Id, user.Email);

        return await MapToResponseAsync(user, organizationId);
    }

    public async Task<UserResponse> UpdateAsync(
        Guid id,
        UpdateUserRequest request,
        Guid organizationId,
        Guid updatedByUserId)
    {
        var user = await _userRepository.GetByIdAsync(id, organizationId);
        if (user == null)
        {
            throw new NotFoundException("Usuario", id);
        }

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Phone = request.Phone;
        if (request.IsActive.HasValue)
        {
            user.IsActive = request.IsActive.Value;
        }
        user.UpdatedAt = DateTime.UtcNow;
        user.UpdatedByUserId = updatedByUserId;

        await _userRepository.UpdateAsync(user);

        _logger.LogInformation("Usuario actualizado: {UserId}", id);

        return await MapToResponseAsync(user, organizationId);
    }

    public async Task DeleteAsync(Guid id, Guid organizationId, Guid deletedByUserId)
    {
        var user = await _userRepository.GetByIdAsync(id, organizationId);
        if (user == null)
        {
            throw new NotFoundException("Usuario", id);
        }

        await _userRepository.SoftDeleteAsync(id, organizationId, deletedByUserId);

        _logger.LogInformation("Usuario eliminado: {UserId}", id);
    }

    #region Helpers

    private async Task<UserResponse> MapToResponseAsync(User user, Guid organizationId)
    {
        // Obtener el rol del usuario si existe
        var userRoles = await _rolePermissionRepository.GetRolesByUserAsync(user.Id, organizationId);
        var firstRole = userRoles.FirstOrDefault();

        return new UserResponse
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Phone = user.Phone,
            IsActive = user.IsActive,
            LastLoginAt = user.LastLoginAt,
            CreatedAt = user.CreatedAt,
            RoleId = firstRole?.Id,
            RoleName = firstRole?.Name ?? "Sin asignar"
        };
    }

    private static (byte[] Hash, byte[] Salt) HashPassword(string password)
    {
        using var hmac = new HMACSHA512();
        var salt = hmac.Key;
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return (hash, salt);
    }

    #endregion
}
