namespace Infrastructure.Repositories;

using Domain.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

/// <summary>
/// Implementación del repositorio de autenticación usando Entity Framework Core
/// </summary>
public class AuthRepository : IAuthRepository
{
    private readonly DevManagerDbContext _context;
    private readonly ILogger<AuthRepository> _logger;

    public AuthRepository(DevManagerDbContext context, ILogger<AuthRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Domain.Entities.IAM.User?> GetUserByEmailAsync(string email, Guid organizationId)
    {
        try
        {
            var efUser = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u =>
                    u.Email == email &&
                    u.OrganizationId == organizationId &&
                    !u.IsDeleted);

            return efUser == null ? null : MapToDomain(efUser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener usuario por email {Email}", email);
            throw;
        }
    }

    public async Task<(Guid OrganizationId, Guid UserId)> RegisterOrganizationAsync(
        Domain.Entities.IAM.Organization organization,
        Domain.Entities.IAM.User adminUser)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // Mapear organización de dominio a EF
            var efOrg = new Infrastructure.Data.Entities.Organization
            {
                Id = organization.Id,
                Name = organization.Name,
                LegalName = organization.LegalName,
                Nit = organization.Nit,
                IsActive = organization.IsActive,
                CreatedAt = organization.CreatedAt
            };

            // Mapear usuario de dominio a EF
            var efUser = new Infrastructure.Data.Entities.User
            {
                Id = adminUser.Id,
                OrganizationId = adminUser.OrganizationId,
                FirstName = adminUser.FirstName,
                LastName = adminUser.LastName,
                Email = adminUser.Email,
                Phone = adminUser.Phone,
                PasswordHash = adminUser.PasswordHash,
                PasswordSalt = adminUser.PasswordSalt,
                IsActive = adminUser.IsActive,
                CreatedAt = adminUser.CreatedAt
            };

            _context.Organizations.Add(efOrg);
            _context.Users.Add(efUser);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return (efOrg.Id, efUser.Id);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error al registrar organización {OrgName}", organization.Name);
            throw;
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
