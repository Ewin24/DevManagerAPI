namespace Infrastructure.Database;

/// <summary>
/// Seam de acceso al esquema SQL Server para que el DatabaseInitializer
/// sea testeable sin una base de datos real.
/// </summary>
public interface IDatabaseSchemaProbe
{
    /// <summary>Indica si existe la tabla de historial de migraciones de EF Core.</summary>
    Task<bool> MigrationHistoryTableExistsAsync(CancellationToken ct = default);

    /// <summary>
    /// Devuelve las tablas de usuario (schema.table), excluyendo __EFMigrationsHistory.
    /// </summary>
    Task<IReadOnlyList<string>> GetUserTablesAsync(CancellationToken ct = default);

    /// <summary>
    /// Elimina todas las tablas indicadas (y sus foreign keys) en una única transacción.
    /// </summary>
    Task DropAllUserTablesAsync(IReadOnlyList<string> tables, CancellationToken ct = default);
}