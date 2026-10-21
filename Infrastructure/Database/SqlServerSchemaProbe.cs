namespace Infrastructure.Database;

using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.Common;

/// <summary>
/// Implementación ADO.NET de <see cref="IDatabaseSchemaProbe"/> sobre la conexión
/// del DbContext. Consulta INFORMATION_SCHEMA para detección y ejecuta un DROP
/// en dos fases (foreign keys y luego tablas) dentro de una única transacción.
/// </summary>
public sealed class SqlServerSchemaProbe(DevManagerDbContext context) : IDatabaseSchemaProbe
{
    public async Task<bool> MigrationHistoryTableExistsAsync(CancellationToken ct = default)
    {
        var connection = context.Database.GetDbConnection();
        var wasOpened = connection.State == ConnectionState.Open;
        if (!wasOpened)
        {
            await connection.OpenAsync(ct);
        }

        try
        {
            await using var command = connection.CreateCommand();
            command.CommandText =
                "SELECT CASE WHEN EXISTS(" +
                "SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '__EFMigrationsHistory') " +
                "THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END";
            var result = await command.ExecuteScalarAsync(ct);
            return result is bool exists && exists;
        }
        finally
        {
            if (!wasOpened && connection.State != ConnectionState.Closed)
            {
                await connection.CloseAsync();
            }
        }
    }

    public async Task<IReadOnlyList<string>> GetUserTablesAsync(CancellationToken ct = default)
    {
        var tables = new List<string>();

        var connection = context.Database.GetDbConnection();
        var wasOpened = connection.State == ConnectionState.Open;
        if (!wasOpened)
        {
            await connection.OpenAsync(ct);
        }

        try
        {
            await using var command = connection.CreateCommand();
            command.CommandText =
                "SELECT TABLE_SCHEMA + '.' + TABLE_NAME " +
                "FROM INFORMATION_SCHEMA.TABLES " +
                "WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME <> '__EFMigrationsHistory' " +
                "ORDER BY 1";

            await using var reader = await command.ExecuteReaderAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                tables.Add(reader.GetString(0));
            }

            return tables;
        }
        finally
        {
            if (!wasOpened && connection.State != ConnectionState.Closed)
            {
                await connection.CloseAsync();
            }
        }
    }

    public async Task DropAllUserTablesAsync(IReadOnlyList<string> tables, CancellationToken ct = default)
    {
        if (tables.Count == 0)
        {
            return;
        }

        var connection = context.Database.GetDbConnection();
        var wasOpened = connection.State == ConnectionState.Open;
        if (!wasOpened)
        {
            await connection.OpenAsync(ct);
        }

        try
        {
            await using var transaction = await connection.BeginTransactionAsync(ct);

            // Fase 1: eliminar todas las foreign keys (excepto las del historial de migraciones)
            await using (var dropFks = connection.CreateCommand())
            {
                dropFks.Transaction = transaction;
                dropFks.CommandText =
                    "DECLARE @sql NVARCHAR(MAX) = N'';" +
                    "SELECT @sql = @sql + N'ALTER TABLE ' + QUOTENAME(OBJECT_SCHEMA_NAME(fk.parent_object_id)) " +
                    "+ N'.' + QUOTENAME(OBJECT_NAME(fk.parent_object_id)) + N' DROP CONSTRAINT ' + QUOTENAME(fk.name) + N';' + CHAR(10) " +
                    "FROM sys.foreign_keys fk " +
                    "WHERE OBJECT_NAME(fk.parent_object_id) <> '__EFMigrationsHistory';" +
                    "IF LEN(@sql) > 0 EXEC sp_executesql @sql;";
                await dropFks.ExecuteNonQueryAsync(ct);
            }

            // Fase 2: eliminar cada tabla de usuario
            foreach (var table in tables)
            {
                await using var dropTable = connection.CreateCommand();
                dropTable.Transaction = transaction;
                dropTable.CommandText = $"DROP TABLE {QuoteQualifiedName(table)}";
                await dropTable.ExecuteNonQueryAsync(ct);
            }

            // Commit explícito: si algo lanza antes, la transacción se descarta sin confirmar (rollback implícito).
            await transaction.CommitAsync(ct);
        }
        finally
        {
            if (!wasOpened && connection.State != ConnectionState.Closed)
            {
                await connection.CloseAsync();
            }
        }
    }

    /// <summary>
    /// Convierte "schema.table" en [schema].[table] a partir de metadatos internos
    /// de INFORMATION_SCHEMA (nombres confiables, no entrada de usuario).
    /// </summary>
    private static string QuoteQualifiedName(string schemaAndTable)
        => string.Join(".", schemaAndTable.Split('.').Select(part => $"[{part.Trim('[', ']')}]"));
}