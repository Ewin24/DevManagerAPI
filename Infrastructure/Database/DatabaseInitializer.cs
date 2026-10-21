namespace Infrastructure.Database;

using Application.Services;
using Domain.Enums;
using Infrastructure.Data;
using Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

/// <summary>
/// Inicializador de base de datos ejecutado en el arranque de la API:
/// detecta esquema legacy (tablas de usuario sin __EFMigrationsHistory) y lo elimina,
/// aplica migraciones de EF Core de forma idempotente y siembra la organización
/// inicial + usuario administrador cuando no existen usuarios. Fail-fast ante errores.
/// </summary>
public class DatabaseInitializer
{
    private readonly DevManagerDbContext _context;
    private readonly IDatabaseSchemaProbe _schemaProbe;
    private readonly ILogger<DatabaseInitializer> _logger;

    public static readonly Guid SeedOrganizationId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    public const string SeedOrganizationName = "TechCorp";
    public const string SeedOrganizationNit = "900123456-7";
    public const string SeedAdminEmail = "admin@techcorp.com";
    public const string SeedAdminPassword = "Password123!";

    public DatabaseInitializer(
        DevManagerDbContext context,
        IDatabaseSchemaProbe schemaProbe,
        ILogger<DatabaseInitializer> logger)
    {
        _context = context;
        _schemaProbe = schemaProbe;
        _logger = logger;
    }

    public async Task InitializeAsync(CancellationToken ct = default)
    {
        try
        {
            await DetectAndCleanLegacySchemaAsync(ct);

            _logger.LogInformation("Aplicando migraciones de EF Core...");
            await ApplyMigrationsAsync(ct);

            await SeedInitialDataAsync(ct);

            _logger.LogInformation("Inicialización de base de datos completada.");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Error al inicializar la base de datos en el arranque");
            throw;
        }
    }

    private async Task DetectAndCleanLegacySchemaAsync(CancellationToken ct)
    {
        var historyExists = await _schemaProbe.MigrationHistoryTableExistsAsync(ct);
        if (historyExists)
        {
            _logger.LogInformation(
                "Esquema migrado detectado (__EFMigrationsHistory presente); no se eliminan tablas.");
            return;
        }

        var tables = await _schemaProbe.GetUserTablesAsync(ct);
        if (tables.Count == 0)
        {
            _logger.LogInformation("Base de datos vacía; no hay esquema legacy que eliminar.");
            return;
        }

        _logger.LogWarning(
            "Esquema legacy detectado sin __EFMigrationsHistory; eliminando {TableCount} tablas: {Tables}",
            tables.Count,
            string.Join(", ", tables));

        await _schemaProbe.DropAllUserTablesAsync(tables, ct);
    }

    /// <summary>
    /// Seam virtual: aplica las migraciones pendientes. Sobre SQL Server equivale
    /// a Database.MigrateAsync (idempotente). Las pruebas lo sobreescriben para
    /// evitar el proveedor InMemory (que no soporta migraciones).
    /// </summary>
    protected virtual Task ApplyMigrationsAsync(CancellationToken ct)
        => _context.Database.MigrateAsync(ct);

    private async Task SeedInitialDataAsync(CancellationToken ct)
    {
        if (await _context.Users.AnyAsync(ct))
        {
            _logger.LogInformation("Usuarios existentes; seeding omitido.");
            return;
        }

        _logger.LogInformation(
            "Sembrando organización inicial y usuario administrador ({AdminEmail})...",
            SeedAdminEmail);

        var (passwordHash, passwordSalt) = PasswordHasher.HashPassword(SeedAdminPassword);

        var organization = new Organization
        {
            Id = SeedOrganizationId,
            Name = SeedOrganizationName,
            Nit = SeedOrganizationNit,
            IsActive = true,
            HierarchyLevel = 0,
            CreatedAt = DateTime.UtcNow
        };

        var admin = new User
        {
            Id = Guid.NewGuid(),
            OrganizationId = SeedOrganizationId,
            FirstName = "Admin",
            LastName = "Admin",
            Email = SeedAdminEmail,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            IsActive = true,
            PersonType = PersonType.Empleado,
            CreatedAt = DateTime.UtcNow
        };

        // Un único SaveChangesAsync: EF lo ejecuta en una transacción implícita (atómico).
        _context.Organizations.Add(organization);
        _context.Users.Add(admin);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Seed completado: organización {OrganizationId} y usuario {AdminEmail}.",
            SeedOrganizationId,
            SeedAdminEmail);
    }
}