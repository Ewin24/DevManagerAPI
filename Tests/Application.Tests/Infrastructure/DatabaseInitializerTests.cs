namespace Application.Tests.Infrastructure;

using Application.Services;
using FluentAssertions;
using global::Infrastructure.Data;
using global::Infrastructure.Data.Entities;
using global::Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

/// <summary>
/// Tests del orquestador DatabaseInitializer (detect → drop → migrate → seed).
/// Usa DbContext InMemory + probe mockeado + subclase que sobreescribe ApplyMigrationsAsync.
/// </summary>
public class DatabaseInitializerTests
{
    private static readonly Guid SeedOrgId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    [Fact]
    public async Task InitializeAsync_WithLegacySchema_DropsTablesAndSeedsOrgAndAdmin()
    {
        // Arrange
        var context = CreateContext();
        var probe = CreateProbe(
            historyExists: false,
            tables: new[] { "iam.Users", "iam.Roles" });
        var initializer = CreateInitializer(context, probe.Object);

        // Act
        await initializer.InitializeAsync();

        // Assert: drop llamado con las tablas legacy exactas
        probe.Verify(p => p.DropAllUserTablesAsync(
            It.Is<IReadOnlyList<string>>(t =>
                t.Count == 2 && t[0] == "iam.Users" && t[1] == "iam.Roles"),
            It.IsAny<CancellationToken>()), Times.Once);

        // Assert: migración aplicada y seed insertó org + admin
        initializer.MigrateCalls.Should().Be(1);
        var org = context.Organizations.Single();
        org.Id.Should().Be(SeedOrgId);
        org.Name.Should().Be("TechCorp");
        org.Nit.Should().Be("900123456-7");
        org.IsActive.Should().BeTrue();

        var admin = context.Users.Single();
        admin.Email.Should().Be("admin@techcorp.com");
        admin.OrganizationId.Should().Be(SeedOrgId);
        admin.IsActive.Should().BeTrue();
        PasswordHasher.VerifyPassword("Password123!", admin.PasswordHash, admin.PasswordSalt)
            .Should().BeTrue();
    }

    [Fact]
    public async Task InitializeAsync_WhenHistoryExists_DoesNotDropTables()
    {
        // Arrange
        var context = CreateContext();
        var probe = CreateProbe(
            historyExists: true,
            tables: new[] { "iam.Users", "iam.Roles" });
        var initializer = CreateInitializer(context, probe.Object);

        // Act
        await initializer.InitializeAsync();

        // Assert: nunca se eliminan tablas en un esquema ya migrado
        probe.Verify(p => p.DropAllUserTablesAsync(
            It.IsAny<IReadOnlyList<string>>(),
            It.IsAny<CancellationToken>()), Times.Never);

        // Assert: migración y seed sí se ejecutan
        initializer.MigrateCalls.Should().Be(1);
        context.Organizations.Should().ContainSingle(o => o.Id == SeedOrgId);
        context.Users.Should().ContainSingle(u => u.Email == "admin@techcorp.com");
    }

    [Fact]
    public async Task InitializeAsync_OnFreshEmptyDatabase_DoesNotDropAndSeeds()
    {
        // Arrange
        var context = CreateContext();
        var probe = CreateProbe(historyExists: false, tables: Array.Empty<string>());
        var initializer = CreateInitializer(context, probe.Object);

        // Act
        await initializer.InitializeAsync();

        // Assert: base vacía → sin drop, migración y seed proceden
        probe.Verify(p => p.DropAllUserTablesAsync(
            It.IsAny<IReadOnlyList<string>>(),
            It.IsAny<CancellationToken>()), Times.Never);
        initializer.MigrateCalls.Should().Be(1);
        context.Organizations.Should().ContainSingle(o => o.Id == SeedOrgId);
        context.Users.Should().ContainSingle(u => u.Email == "admin@techcorp.com");
    }

    [Fact]
    public async Task InitializeAsync_WhenUsersAlreadyExist_SkipsSeed()
    {
        // Arrange
        var context = CreateContext();
        context.Users.Add(new User
        {
            Id = Guid.NewGuid(),
            OrganizationId = SeedOrgId,
            FirstName = "Existing",
            LastName = "User",
            Email = "existing@techcorp.com",
            PasswordHash = [1, 2, 3],
            PasswordSalt = [4, 5, 6],
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });
        await context.SaveChangesAsync();

        var probe = CreateProbe(historyExists: true, tables: new[] { "iam.Users" });
        var initializer = CreateInitializer(context, probe.Object);

        // Act
        await initializer.InitializeAsync();

        // Assert: seed idempotente → no se crea org ni usuario duplicado
        context.Organizations.Should().BeEmpty();
        context.Users.Should().ContainSingle(u => u.Email == "existing@techcorp.com");
        context.Users.Should().NotContain(u => u.Email == "admin@techcorp.com");
    }

    [Fact]
    public async Task InitializeAsync_WhenProbeThrows_RethrowsException()
    {
        // Arrange
        var context = CreateContext();
        var probe = new Mock<IDatabaseSchemaProbe>();
        probe.Setup(p => p.MigrationHistoryTableExistsAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("connection failed"));
        var initializer = CreateInitializer(context, probe.Object);

        // Act
        var act = () => initializer.InitializeAsync();

        // Assert: fail-fast — la excepción se propaga al llamador
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("connection failed");
    }

    private static DevManagerDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<DevManagerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new DevManagerDbContext(options);
    }

    private static Mock<IDatabaseSchemaProbe> CreateProbe(
        bool historyExists,
        IReadOnlyList<string>? tables = null)
    {
        var probe = new Mock<IDatabaseSchemaProbe>();
        probe.Setup(p => p.MigrationHistoryTableExistsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(historyExists);
        probe.Setup(p => p.GetUserTablesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(tables ?? Array.Empty<string>());
        return probe;
    }

    private static TestableDatabaseInitializer CreateInitializer(
        DevManagerDbContext context,
        IDatabaseSchemaProbe probe)
        => new(context, probe, NullLogger<DatabaseInitializer>.Instance);

    /// <summary>
    /// Subclase que evita MigrateAsync sobre InMemory (no soportado) y cuenta las llamadas.
    /// </summary>
    private sealed class TestableDatabaseInitializer(
        DevManagerDbContext context,
        IDatabaseSchemaProbe probe,
        ILogger<DatabaseInitializer> logger) : DatabaseInitializer(context, probe, logger)
    {
        public int MigrateCalls { get; private set; }

        protected override Task ApplyMigrationsAsync(CancellationToken ct)
        {
            MigrateCalls++;
            return Task.CompletedTask;
        }
    }
}