using System.Security.Cryptography;
using System.Text;
using Application.Common.Exceptions;
using Application.DTOs.Auth;
using Application.Interfaces;
using Application.Services;
using Domain.Entities.IAM;
using Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Tests;

public class AuthServiceTests
{
    private const string Email = "dev@acme.com";
    private const string Password = "Password123!";

    private readonly Mock<IAuthRepository> _authRepo = new();
    private readonly Mock<IRolePermissionRepository> _roleRepo = new();
    private readonly Mock<ITokenService> _tokenService = new();
    private IEnumerable<string>? _capturedRoles;

    private AuthService CreateService()
    {
        _roleRepo.Setup(r => r.GetRolesByUserAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(new[] { new Role { Name = "Admin" } });
        _tokenService.Setup(t => t.GenerateToken(It.IsAny<User>(), It.IsAny<IEnumerable<string>?>()))
            .Callback<User, IEnumerable<string>?>((_, roles) => _capturedRoles = roles?.ToList())
            .Returns("token");

        return new AuthService(_authRepo.Object, Mock.Of<IUserRepository>(), _roleRepo.Object,
            _tokenService.Object, NullLogger<AuthService>.Instance);
    }

    private static User NewUser(Guid orgId)
    {
        using var hmac = new HMACSHA512();
        return new User
        {
            Id = Guid.NewGuid(), OrganizationId = orgId, Email = Email, FirstName = "Dev", LastName = "User",
            IsActive = true, PasswordSalt = hmac.Key, PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(Password))
        };
    }

    [Fact]
    public async Task Login_SingleOrg_ResolvesTenantFromEmail_AndPassesRoles()
    {
        var orgId = Guid.NewGuid();
        _authRepo.Setup(r => r.GetUsersByEmailAsync(Email)).ReturnsAsync(new[] { NewUser(orgId) });

        var result = await CreateService().LoginAsync(new LoginRequest { Email = Email, Password = Password });

        Assert.Equal(orgId, result.OrganizationId);
        Assert.Equal(new[] { "Admin" }, _capturedRoles);
    }

    [Fact]
    public async Task Login_MultiOrg_WithoutOrganizationId_Throws()
    {
        _authRepo.Setup(r => r.GetUsersByEmailAsync(Email))
            .ReturnsAsync(new[] { NewUser(Guid.NewGuid()), NewUser(Guid.NewGuid()) });

        await Assert.ThrowsAsync<BusinessValidationException>(() =>
            CreateService().LoginAsync(new LoginRequest { Email = Email, Password = Password }));
    }

    [Fact]
    public async Task Login_MultiOrg_WithOrganizationId_PicksThatOrg()
    {
        var targetOrg = Guid.NewGuid();
        _authRepo.Setup(r => r.GetUsersByEmailAsync(Email))
            .ReturnsAsync(new[] { NewUser(Guid.NewGuid()), NewUser(targetOrg) });

        var result = await CreateService().LoginAsync(
            new LoginRequest { Email = Email, Password = Password, OrganizationId = targetOrg });

        Assert.Equal(targetOrg, result.OrganizationId);
    }

    [Fact]
    public async Task RegisterOrganization_DuplicateNit_ThrowsConflict()
    {
        _authRepo.Setup(r => r.NitExistsAsync("900123")).ReturnsAsync(true);

        await Assert.ThrowsAsync<ConflictException>(() => CreateService().RegisterOrganizationAsync(
            new RegisterOrganizationRequest
            {
                OrganizationName = "Acme", Nit = "900123", FirstName = "A", LastName = "B",
                Email = Email, Password = Password
            }));
    }
}
