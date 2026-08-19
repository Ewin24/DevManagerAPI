namespace Application.Services.Reportes;

using Application.DTOs.Reportes;
using Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;

/// <summary>
/// Implementación del generador de reportes para Supersolidaria
/// Compila Balance Social, estadísticas de asociados, cumplimiento SST y normativo
/// </summary>
public class ReportGeneratorService : IReportGeneratorService
{
    private readonly ILogger<ReportGeneratorService> _logger;
    private readonly List<ReporteSupersolidariaDto> _reportesStore = new();

    public ReportGeneratorService(ILogger<ReportGeneratorService> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc/>
    public Task<ReporteSupersolidariaDto> GenerarReporteAsync(
        Guid organizationId, DateTime periodo, string tipoReporte)
    {
        _logger.LogInformation(
            "Generando reporte {Tipo} para organización {OrgId}, período {Periodo}",
            tipoReporte, organizationId, periodo);

        // Compilar Balance Social (indicadores simulados)
        var balanceSocial = new
        {
            GobernanzaDemocratica = new { cobertura = 0.75, asambleas = 2, participacion = 0.68 },
            SatisfaccionNecesidades = new { quejasResueltas = 45, satisfaccion = 0.82 },
            CompromisoComunitario = new { inversionComunitaria = 12_500_000m, proyectos = 3 },
            EducacionEInformacion = new { cobertura = 0.60, horasPromedio = 22.5m },
            EticaYTransparencia = new { codigoEtica = true, informesTrimestrales = 4 }
        };

        // Estadísticas de asociados simuladas
        var asociados = new
        {
            totalAsociados = 150,
            activos = 120,
            suspendidos = 5,
            retirados = 25,
            nuevosEnPeriodo = 8,
            mujeres = 65,
            hombres = 85,
            aportePromedio = 450_000m
        };

        // Cumplimiento SST y normativo
        var cumplimiento = new
        {
            sst = new
            {
                arlVigente = true,
                examenesRealizados = 45,
                examenesPendientes = 12,
                accidentesEnPeriodo = 3,
                investigacionesCompletadas = 3,
                matrizRiesgosActualizada = true
            },
            normativo = new
            {
                excedentesDistribuidos = true,
                educacionCumple20Porciento = true,
                habeasDataAlDia = true,
                estatutosActualizados = true,
                revisorFiscalDesignado = true
            }
        };

        var balanceSocialJson = JsonSerializer.Serialize(balanceSocial);
        var asociadosJson = JsonSerializer.Serialize(asociados);
        var cumplimientoJson = JsonSerializer.Serialize(cumplimiento);

        // Verificar si ya existe reporte para el mismo período
        var existing = _reportesStore.FirstOrDefault(r =>
            r.OrganizationId == organizationId && r.Periodo == periodo);

        if (existing != null)
        {
            var updated = existing with
            {
                BalanceSocialJson = balanceSocialJson,
                AsociadosJson = asociadosJson,
                CumplimientoJson = cumplimientoJson,
                TipoReporte = tipoReporte,
                Enviado = false,
                FechaEnvio = null
            };

            var index = _reportesStore.IndexOf(existing);
            _reportesStore[index] = updated;
            return Task.FromResult(updated);
        }

        var reporte = new ReporteSupersolidariaDto
        {
            Id = Guid.NewGuid(),
            OrganizationId = organizationId,
            Periodo = periodo,
            BalanceSocialJson = balanceSocialJson,
            AsociadosJson = asociadosJson,
            CumplimientoJson = cumplimientoJson,
            TipoReporte = tipoReporte,
            Enviado = false,
            CreatedAt = DateTime.UtcNow
        };

        _reportesStore.Add(reporte);
        return Task.FromResult(reporte);
    }

    /// <inheritdoc/>
    public Task<ReporteSupersolidariaDto?> GetReporteByPeriodoAsync(Guid organizationId, DateTime periodo)
    {
        var result = _reportesStore.FirstOrDefault(r =>
            r.OrganizationId == organizationId && r.Periodo == periodo);

        return Task.FromResult(result);
    }

    /// <inheritdoc/>
    public Task<List<ReporteSupersolidariaDto>> GetReportesByOrganizacionAsync(Guid organizationId)
    {
        var result = _reportesStore
            .Where(r => r.OrganizationId == organizationId)
            .OrderByDescending(r => r.Periodo)
            .ToList();

        return Task.FromResult(result);
    }

    /// <inheritdoc/>
    public Task<ReporteSupersolidariaDto> MarcarEnviadoAsync(Guid reporteId)
    {
        _logger.LogInformation("Marcando reporte {ReporteId} como enviado a Supersolidaria", reporteId);

        var existing = _reportesStore.FirstOrDefault(r => r.Id == reporteId);
        if (existing == null)
            throw new KeyNotFoundException($"Reporte {reporteId} no encontrado");

        var updated = existing with
        {
            Enviado = true,
            FechaEnvio = DateTime.UtcNow
        };

        var index = _reportesStore.IndexOf(existing);
        _reportesStore[index] = updated;

        return Task.FromResult(updated);
    }
}
