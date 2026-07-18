namespace Infrastructure.Repositories;

using Domain.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

/// <summary>
/// Implementación del repositorio de usuarios usando Entity Framework Core
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly DevManagerDbContext _context;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(DevManagerDbContext context, ILogger<UserRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Domain.Entities.IAM.User>> GetAllAsync(Guid organizationId)
    {
        var efUsers = await _context.Users
            .AsNoTracking()
            .Where(u => u.OrganizationId == organizationId && !u.IsDeleted)
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();

        return efUsers.Select(MapToDomain);
    }

    public async Task<Domain.Entities.IAM.User?> GetByIdAsync(Guid id, Guid organizationId)
    {
        var efUser = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u =>
                u.Id == id &&
                u.OrganizationId == organizationId &&
                !u.IsDeleted);

        return efUser == null ? null : MapToDomain(efUser);
    }

    public async Task<bool> EmailExistsAsync(string email, Guid organizationId, Guid? excludeUserId = null)
    {
        return await _context.Users
            .AnyAsync(u =>
                u.Email == email &&
                u.OrganizationId == organizationId &&
                !u.IsDeleted &&
                (excludeUserId == null || u.Id != excludeUserId));
    }

    public async Task<Guid> CreateAsync(Domain.Entities.IAM.User user)
    {
        var efUser = new Infrastructure.Data.Entities.User
        {
            Id = user.Id,
            OrganizationId = user.OrganizationId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Phone = user.Phone,
            PasswordHash = user.PasswordHash,
            PasswordSalt = user.PasswordSalt,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            CreatedByUserId = user.CreatedByUserId
        };

        _context.Users.Add(efUser);
        await _context.SaveChangesAsync();

        return efUser.Id;
    }

    public async Task<bool> UpdateAsync(Domain.Entities.IAM.User user)
    {
        var efUser = await _context.Users
            .FirstOrDefaultAsync(u =>
                u.Id == user.Id &&
                u.OrganizationId == user.OrganizationId &&
                !u.IsDeleted);

        if (efUser == null)
            return false;

        efUser.FirstName = user.FirstName;
        efUser.LastName = user.LastName;
        efUser.Phone = user.Phone;
        efUser.IsActive = user.IsActive;
        efUser.UpdatedAt = DateTime.UtcNow;
        efUser.UpdatedByUserId = user.UpdatedByUserId;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SoftDeleteAsync(Guid id, Guid organizationId, Guid deletedByUserId)
    {
        var efUser = await _context.Users
            .FirstOrDefaultAsync(u =>
                u.Id == id &&
                u.OrganizationId == organizationId &&
                !u.IsDeleted);

        if (efUser == null)
            return false;

        efUser.IsDeleted = true;
        efUser.DeletedAt = DateTime.UtcNow;
        efUser.DeletedByUserId = deletedByUserId;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task UpdateLastLoginAsync(Guid userId)
    {
        var efUser = await _context.Users.FindAsync(userId);
        if (efUser != null)
        {
            efUser.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    #region Mapping Helpers

    private Domain.Entities.IAM.User MapToDomain(Infrastructure.Data.Entities.User efUser)
    {
        return new Domain.Entities.IAM.User
        {
            Id = efUser.Id,
            OrganizationId = efUser.OrganizationId,
            FirstName = efUser.FirstName,
            LastName = efUser.LastName,
            Email = efUser.Email,
            Phone = efUser.Phone,
            PasswordHash = efUser.PasswordHash,
            PasswordSalt = efUser.PasswordSalt,
            IsActive = efUser.IsActive,
            LastLoginAt = efUser.LastLoginAt,
            CreatedAt = efUser.CreatedAt,
            CreatedByUserId = efUser.CreatedByUserId,
            UpdatedAt = efUser.UpdatedAt,
            UpdatedByUserId = efUser.UpdatedByUserId,
            IsDeleted = efUser.IsDeleted,
            DeletedAt = efUser.DeletedAt,
            DeletedByUserId = efUser.DeletedByUserId
        };
    }

    #endregion
}
