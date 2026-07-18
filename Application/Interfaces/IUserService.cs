namespace Application.Interfaces;

using Application.DTOs.Users;

/// <summary>
/// Servicio de gestión de usuarios
/// </summary>
public interface IUserService
{
    Task<IEnumerable<UserResponse>> GetAllAsync(Guid organizationId);
    Task<UserResponse> GetByIdAsync(Guid id, Guid organizationId);
    Task<UserResponse> CreateAsync(CreateUserRequest request, Guid organizationId, Guid createdByUserId);
    Task<UserResponse> UpdateAsync(Guid id, UpdateUserRequest request, Guid organizationId, Guid updatedByUserId);
    Task DeleteAsync(Guid id, Guid organizationId, Guid deletedByUserId);
}
