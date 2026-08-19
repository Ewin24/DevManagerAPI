namespace Application.Services.BalanceSocial;

using Application.DTOs.BalanceSocial;
using Application.Interfaces;
using Microsoft.Extensions.Logging;

/// <summary>
/// Implementación del servicio de balance social
/// Calcula indicadores de gestión social por asociado, incluyendo
/// cumplimiento educativo (20hr mínimas), participación y aportes
/// </summary>
public class BalanceSocialService : IBalanceSocialService
{
    private readonly ILogger<BalanceSocialService> _logger;
    private readonly List<IndicadorBalanceSocialDto> _indicadoresStore = new();

    private const int HorasMinimasEducacion = 20;

    public BalanceSocialService(ILogger<BalanceSocialService> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc/>
    public Task<IndicadorBalanceSocialDto?> GetIndicadorAsync(Guid asociadoId, int anio)
    {
        _logger.LogInformation(
            "Obteniendo indicador de balance social para asociado {AsociadoId}, año {Anio}",
            asociadoId, anio);

        var indicador = _indicadoresStore
            .FirstOrDefault(i => i.AsociadoId == asociadoId && i.Anio == anio);

        return Task.FromResult(indicador);
    }

    /// <inheritdoc/>
    public Task<IndicadorBalanceSocialDto> CalcularIndicadorAsync(Guid asociadoId, Guid organizationId, int anio)
    {
        _logger.LogInformation(
            "Calculando indicador de balance social para asociado {AsociadoId}, año {Anio}",
            asociadoId, anio);

        // En una implementación real, estos valores se calcularían
        // a partir de datos de educación, participación y aportes
        var horasEducacion = 0;
        var participacionAsambleas = 0;
        var participacionComites = 0;
        var aportesSociales = 0m;
        var beneficiosRecibidos = 0m;

        var cumpleEducacion = horasEducacion >= HorasMinimasEducacion;

        // Índice compuesto (0-100): pondera educación 40%, participación 30%, aportes 30%
        var puntajeEducacion = Math.Min((decimal)horasEducacion / HorasMinimasEducacion * 40, 40);
        var puntajeParticipacion = Math.Min((participacionAsambleas + participacionComites) * 10, 30);
        var puntajeAportes = Math.Min(aportesSociales > 0 ? 30 : 0, 30);
        var indiceCompuesto = Math.Round(puntajeEducacion + puntajeParticipacion + puntajeAportes, 2);

        var existing = _indicadoresStore
            .FirstOrDefault(i => i.AsociadoId == asociadoId && i.Anio == anio);

        var indicador = new IndicadorBalanceSocialDto
        {
            Id = existing?.Id ?? Guid.NewGuid(),
            AsociadoId = asociadoId,
            OrganizationId = organizationId,
            Anio = anio,
            HorasEducacion = horasEducacion,
            ParticipacionAsambleas = participacionAsambleas,
            ParticipacionComites = participacionComites,
            AportesSociales = aportesSociales,
            BeneficiosRecibidos = beneficiosRecibidos,
            CumpleEducacion = cumpleEducacion,
            IndiceBalanceSocial = indiceCompuesto
        };

        if (existing != null)
        {
            var index = _indicadoresStore.IndexOf(existing);
            _indicadoresStore[index] = indicador;
        }
        else
        {
            _indicadoresStore.Add(indicador);
        }

        return Task.FromResult(indicador);
    }

    /// <inheritdoc/>
    public Task<List<IndicadorBalanceSocialDto>> GetIndicadoresByOrganizacionAsync(Guid organizationId, int anio)
    {
        _logger.LogInformation(
            "Obteniendo indicadores de balance social de organización {OrgId}, año {Anio}",
            organizationId, anio);

        var result = _indicadoresStore
            .Where(i => i.OrganizationId == organizationId && i.Anio == anio)
            .ToList();

        return Task.FromResult(result);
    }

    /// <inheritdoc/>
    public Task<List<IndicadorBalanceSocialDto>> GetNoCumplenEducacionAsync(Guid organizationId, int anio)
    {
        _logger.LogInformation(
            "Obteniendo asociados que NO cumplen educación mínima en organización {OrgId}, año {Anio}",
            organizationId, anio);

        var result = _indicadoresStore
            .Where(i => i.OrganizationId == organizationId
                        && i.Anio == anio
                        && !i.CumpleEducacion)
            .ToList();

        return Task.FromResult(result);
    }
}
