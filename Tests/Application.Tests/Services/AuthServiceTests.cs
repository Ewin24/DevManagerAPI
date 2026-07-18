namespace Application.Tests.Services;

using Application.Common.Exceptions;
using Application.DTOs.Auth;
using Application.Interfaces;
using Application.Services;
using Domain.Entities.IAM;
using Domain.Interfaces.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using Xunit;

/// <summary>
/// Tests unitarios para AuthService (Fase 0 — Fundación)
/// </summary>
public class AuthServiceTests
{
    private readonly Mock<IAuthRepository> _authRepoMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<ILogger<AuthService>> _loggerMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _authRepoMock = new Mock<IAuthRepository>();
        _userRepoMock = new Mock<IUserRepository>();
        _tokenServiceMock = new Mock<ITokenService>();
        _loggerMock = new Mock<ILogger<AuthService>>();

        _authService = new AuthService(
            _authRepoMock.Object,
            _userRepoMock.Object,
            _tokenServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task RegisterOrganizationAsync_WithValidData_ReturnsSuccessResponse()
    {
        // Arrange
        var request = new RegisterOrganizationRequest
        {
            OrganizationName = "Cooperativa Test",
            LegalName = "Cooperativa Test Ltda.",
            Nit = "900123456-7",
            FirstName = "Carlos",
            LastName = "Pérez",
            Email = "carlos@cooperativa.com",
            Phone = "3001234567",
            Password = "Segura123!"
        };

        var expectedOrgId = Guid.NewGuid();
        var expectedUserId = Guid.NewGuid();

        _authRepoMock
            .Setup(r => r.RegisterOrganizationAsync(
                It.IsAny<Organization>(),
                It.IsAny<User>()))
            .ReturnsAsync((expectedOrgId, expectedUserId));

        // Act
        var result = await _authService.RegisterOrganizationAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.OrganizationId.Should().Be(expectedOrgId);
        result.AdminUserId.Should().Be(expectedUserId);
        _authRepoMock.Verify(
            r => r.RegisterOrganizationAsync(
                It.IsAny<Organization>(),
                It.IsAny<User>()),
            Times.Once);
    }

    [Fact]
    public async Task RegisterOrganizationAsync_WithDuplicateEmail_ThrowsException()
    {
        // Arrange
        var request = new RegisterOrganizationRequest
        {
            OrganizationName = "Cooperativa Duplicada",
            LegalName = "Cooperativa Duplicada Ltda.",
            Nit = "900987654-3",
            FirstName = "Ana",
            LastName = "García",
            Email = "ana@duplicada.com",
            Phone = "3109876543",
            Password = "Clave456!"
        };

        _authRepoMock
            .Setup(r => r.RegisterOrganizationAsync(
                It.IsAny<Organization>(),
                It.IsAny<User>()))
            .ThrowsAsync(new DuplicateNameException(
                "Ya existe un usuario con el email 'ana@duplicada.com'"));

        // Act
        var act = () => _authService.RegisterOrganizationAsync(request);

        // Assert
        await act.Should().ThrowAsync<DuplicateNameException>()
            .WithMessage("*email*duplicada*");
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsJwtToken()
    {
        // Arrange
        var password = "MiClave123!";
        var (hash, salt) = ComputePasswordHash(password);

        var userId = Guid.NewGuid();
        var orgId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            OrganizationId = orgId,
            FirstName = "María",
            LastName = "López",
            Email = "maria@cooperativa.com",
            PasswordHash = hash,
            PasswordSalt = salt,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var request = new LoginRequest
        {
            Email = "maria@cooperativa.com",
            Password = password
        };

        const string expectedToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.mock-token";

        _authRepoMock
            .Setup(r => r.GetUserByEmailAsync(request.Email, It.IsAny<Guid>()))
            .ReturnsAsync(user);

        _tokenServiceMock
            .Setup(t => t.GenerateToken(user))
            .Returns(expectedToken);

        _userRepoMock
            .Setup(r => r.UpdateLastLoginAsync(userId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be(userId);
        result.Email.Should().Be("maria@cooperativa.com");
        result.FirstName.Should().Be("María");
        result.LastName.Should().Be("López");
        result.Token.Should().Be(expectedToken);
        result.OrganizationId.Should().Be(orgId);
        result.ExpiresAt.Should().BeAfter(DateTime.UtcNow);

        _userRepoMock.Verify(r => r.UpdateLastLoginAsync(userId), Times.Once);
    }

    /// <summary>
    /// Reproduce the exact same HMACSHA512 hashing used by AuthService
    /// so VerifyPassword passes in the SUT.
    /// </summary>
    private static (byte[] Hash, byte[] Salt) ComputePasswordHash(string password)
    {
        using var hmac = new HMACSHA512();
        var salt = hmac.Key;
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return (hash, salt);
    }
}
