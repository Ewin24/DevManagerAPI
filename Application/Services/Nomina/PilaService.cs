namespace Application.Services.Nomina;

using Application.DTOs.Nomina;
using Application.Interfaces;
using Domain.Enums;
using Microsoft.Extensions.Logging;

/// <summary>
/// Implementación del servicio PILA para asociados tipo CTA
/// Tasas según Decreto 2150/2017:
/// - EPS: 12.5%
/// - Pensión: 16%
/// - ARL: 0.522% (riesgo 1) a 6.96% (riesgo 5)
/// </summary>
public class PilaService : IPilaService
{
    private readonly ILogger<PilaService> _logger;

    // Tasas PILA tipo 51 (Independiente CTA)
    private const decimal TasaEPS = 0.125m;
    private const decimal TasaPension = 0.16m;

    // Tasas ARL por nivel de riesgo (Decreto 2150/2017, tabla)
    private static readonly decimal[] TasasARL = { 0m, 0.00522m, 0.01044m, 0.02436m, 0.04350m, 0.06960m };

    public PilaService(ILogger<PilaService> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc/>
    public Task<PilaAporteDto> CalcularAportesAsync(Guid asociadoId, decimal ingresos, int nivelRiesgoARL)
    {
        _logger.LogInformation(
            "Calculando aportes PILA para asociado {AsociadoId}, ingresos {Ingresos}, riesgo ARL {Riesgo}",
            asociadoId, ingresos, nivelRiesgoARL);

        var tasaARL = ObtenerTasaARL(nivelRiesgoARL);

        var aporteEPS = Math.Round(ingresos * TasaEPS, 2, MidpointRounding.AwayFromZero);
        var aportePension = Math.Round(ingresos * TasaPension, 2, MidpointRounding.AwayFromZero);
        var aporteARL = Math.Round(ingresos * tasaARL, 2, MidpointRounding.AwayFromZero);
        var total = aporteEPS + aportePension + aporteARL;

        var dto = new PilaAporteDto
        {
            Id = Guid.NewGuid(),
            AsociadoId = asociadoId,
            OrganizationId = Guid.Empty, // Se asigna en contexto real
            Periodo = DateTime.UtcNow,
            TipoAportante = PilaTipoAportante.Independiente,
            IngresoBase = ingresos,
            AporteEPS = aporteEPS,
            AportePension = aportePension,
            AporteARL = aporteARL,
            Total = total
        };

        return Task.FromResult(dto);
    }

    /// <inheritdoc/>
    public Task<List<PilaAporteDto>> GenerarPlanillaAsync(int mes, int anio, Guid organizationId)
    {
        _logger.LogInformation(
            "Generando planilla PILA para organización {OrgId}, período {Mes}/{Anio}",
            organizationId, mes, anio);

        // Implementación base — en producción consultaría los asociados activos
        return Task.FromResult(new List<PilaAporteDto>());
    }

    /// <summary>
    /// Obtiene la tasa ARL según el nivel de riesgo (1-5)
    /// </summary>
    private static decimal ObtenerTasaARL(int nivelRiesgo)
    {
        if (nivelRiesgo < 1 || nivelRiesgo > 5)
        {
            return TasasARL[1]; // Default riesgo 1
        }

        return TasasARL[nivelRiesgo];
    }
}
