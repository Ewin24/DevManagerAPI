using Application.Interfaces;
using Infrastructure.Data;
using Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Infrastructure.BackgroundServices;

/// <summary>
/// Servicio en segundo plano que guarda un snapshot diario de reportes por organización
/// en reporting.ReportSnapshots (uno por organización y día).
/// </summary>
public class ReportSnapshotGeneratorService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ReportSnapshotGeneratorService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromHours(24); // Diario

    public ReportSnapshotGeneratorService(
        IServiceProvider serviceProvider,
        ILogger<ReportSnapshotGeneratorService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ReportSnapshotGenerator iniciado");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await GenerateSnapshotsAsync(stoppingToken);
                await Task.Delay(_interval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("ReportSnapshotGenerator detenido");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando snapshots");
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }

    private async Task GenerateSnapshotsAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DevManagerDbContext>();
        var reportsService = scope.ServiceProvider.GetRequiredService<IReportsService>();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var organizationIds = await context.Organizations
            .AsNoTracking()
            .Where(o => o.IsActive && !o.IsDeleted)
            .Select(o => o.Id)
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Generando snapshots de reporting para {Count} organizaciones...", organizationIds.Count);

        foreach (var organizationId in organizationIds)
        {
            // Índice único (OrganizationId, SnapshotDate): un snapshot por día
            var exists = await context.ReportSnapshots
                .AnyAsync(s => s.OrganizationId == organizationId && s.SnapshotDate == today, cancellationToken);

            if (exists)
                continue;

            try
            {
                // Solo métricas calculadas localmente (sin llamadas a Gemini)
                var payload = new
                {
                    skillsDistribution = await reportsService.GetSkillsDistributionAsync(organizationId),
                    projectMetrics = await reportsService.GetProjectMetricsAsync(organizationId),
                    employeeUtilization = await reportsService.GetEmployeeUtilizationAsync(organizationId),
                    departmentMetrics = await reportsService.GetDepartmentMetricsAsync(organizationId)
                };

                context.ReportSnapshots.Add(new ReportSnapshot
                {
                    Id = Guid.NewGuid(),
                    OrganizationId = organizationId,
                    SnapshotDate = today,
                    JsonPayload = JsonSerializer.Serialize(payload),
                    CreatedAt = DateTime.UtcNow
                });

                await context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Error generando snapshot para organización {OrganizationId}", organizationId);
                context.ChangeTracker.Clear();
            }
        }

        _logger.LogInformation("Snapshots generados exitosamente");
    }
}
