namespace Application.Services.Reportes;

using Application.DTOs.Reportes;
using Application.Interfaces;
using Domain.Entities.Reportes;
using Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using System.Text.Json;

/// <summary>
/// Implementación del generador de reportes para Supersolidaria
/// Compila Balance Social, estadísticas de asociados, cumplimiento SST y normativo
/// </summary>
public class ReportGeneratorService : IReportGeneratorService
{
    private readonly IReporteSupersolidariaRepository _repository;
    private readonly ILogger<ReportGeneratorService> _logger;

    public ReportGeneratorService(IReporteSupersolidariaRepository repository, ILogger<ReportGeneratorService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<ReporteSupersolidariaDto> GenerarReporteAsync(
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
        var existing = await _repository.GetByOrganizationAndPeriodoAsync(organizationId, periodo);

        if (existing != null)
        {
            existing.BalanceSocialJson = balanceSocialJson;
            existing.AsociadosJson = asociadosJson;
            existing.CumplimientoJson = cumplimientoJson;
            existing.TipoReporte = tipoReporte;
            existing.Enviado = false;
            existing.FechaEnvio = null;

            var updated = await _repository.UpdateAsync(existing);
            return MapToDto(updated);
        }

        var reporte = new ReporteSupersolidaria
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

        var creado = await _repository.CreateAsync(reporte);
        return MapToDto(creado);
    }

    /// <inheritdoc/>
    public async Task<ReporteSupersolidariaDto?> GetReporteByPeriodoAsync(Guid organizationId, DateTime periodo)
    {
        var result = await _repository.GetByOrganizationAndPeriodoAsync(organizationId, periodo);
        return result != null ? MapToDto(result) : null;
    }

    /// <inheritdoc/>
    public async Task<List<ReporteSupersolidariaDto>> GetReportesByOrganizacionAsync(Guid organizationId)
    {
        var result = await _repository.GetByOrganizationAsync(organizationId);
        return result.Select(MapToDto).ToList();
    }

    /// <inheritdoc/>
    public async Task<ReporteSupersolidariaDto> MarcarEnviadoAsync(Guid reporteId)
    {
        _logger.LogInformation("Marcando reporte {ReporteId} como enviado a Supersolidaria", reporteId);

        var existing = await _repository.GetByIdAsync(reporteId);
        if (existing == null)
            throw new KeyNotFoundException($"Reporte {reporteId} no encontrado");

        existing.Enviado = true;
        existing.FechaEnvio = DateTime.UtcNow;

        var updated = await _repository.UpdateAsync(existing);
        return MapToDto(updated);
    }

    private static ReporteSupersolidariaDto MapToDto(ReporteSupersolidaria r) => new()
    {
        Id = r.Id,
        OrganizationId = r.OrganizationId,
        Periodo = r.Periodo,
        BalanceSocialJson = r.BalanceSocialJson,
        AsociadosJson = r.AsociadosJson,
        CumplimientoJson = r.CumplimientoJson,
        TipoReporte = r.TipoReporte,
        Enviado = r.Enviado,
        FechaEnvio = r.FechaEnvio,
        Observaciones = r.Observaciones,
        CreatedAt = r.CreatedAt
    };
}