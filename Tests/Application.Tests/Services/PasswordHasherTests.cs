namespace Application.Tests.Services;

using Application.Services;
using FluentAssertions;
using Xunit;

/// <summary>
/// Tests unitarios para PasswordHasher (esquema HMACSHA512 compartido).
/// Pines contra las constantes históricas de Infrastructure/Database/Seeders/Seeder.sql.
/// </summary>
public class PasswordHasherTests
{
    // Constantes históricas de Seeder.sql (hash y salt de "Password123!")
    // Salt:  DECLARE @PasswordSalt VARBINARY(512) = 0x73BBB749...F2838F2;
    // Hash:  DECLARE @PasswordHash VARBINARY(512) = 0x3E4657E2...F2D16D0;
    private static readonly byte[] LegacySalt = Convert.FromHexString(
        "73BBB749E8FADB426B239CCCCAC1E1E34D972F1153C8F9ED075746CD22F8A7A2E93B014F23B50C09F22A67223888CC145BA8B242EFF90B6DE40114E96003E52E64B17AC022084DEA2DD4958E74BCB4A59A8E0132FD2B8F4B6366701F8B21397BDB9C19376037D5D197CE43377648AF219BEEFDE214E1FFE0EBD0B1B55F2838F2");

    private static readonly byte[] LegacyHash = Convert.FromHexString(
        "3E4657E2784DE998AAABD30A65D3581085979A1967FB0349148C005EE4EBA5F8ABB2CECF645DB5A9063463F89E97B7C7120EBB41A916B904FCC98F790F2D16D0");

    [Fact]
    public void HashPassword_ThenVerifyPassword_WithSamePassword_ReturnsTrue()
    {
        // Arrange
        const string password = "Password123!";

        // Act
        var (hash, salt) = PasswordHasher.HashPassword(password);
        var verified = PasswordHasher.VerifyPassword(password, hash, salt);

        // Assert
        verified.Should().BeTrue();
        hash.Should().NotBeEmpty();
        salt.Should().NotBeEmpty();
    }

    [Fact]
    public void VerifyPassword_WithWrongPassword_ReturnsFalse()
    {
        // Arrange
        const string password = "Password123!";

        // Act
        var (hash, salt) = PasswordHasher.HashPassword(password);
        var verified = PasswordHasher.VerifyPassword("WrongPassword!", hash, salt);

        // Assert
        verified.Should().BeFalse();
    }

    [Fact]
    public void VerifyPassword_WithLegacySeederSqlConstants_ReturnsTrue()
    {
        // Act: los mismos hash/salt que Seeder.sql usó para "Password123!"
        var verified = PasswordHasher.VerifyPassword("Password123!", LegacyHash, LegacySalt);

        // Assert: el runtime debe reproducir el esquema histórico de hashing
        verified.Should().BeTrue();
    }
}