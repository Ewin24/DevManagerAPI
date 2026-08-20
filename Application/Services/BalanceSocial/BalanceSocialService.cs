namespace Application.Services.BalanceSocial;

using Application.DTOs.BalanceSocial;
using Application.Interfaces;
using Domain.Entities.BalanceSocial;
using Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

/// <summary>
/// Implementación del servicio de balance social
/// Calcula indicadores de gestión social por asociado, incluyendo
/// cumplimiento educativo (20hr mínimas), participación y aportes
/// </summary>
public class BalanceSocialService : IBalanceSocialService
{
    private readonly IIndicadorBalanceSocialRepository _repository;
    private readonly ILogger<BalanceSocialService> _logger;

    private const int HorasMinimasEducacion = 20;

    public BalanceSocialService(IIndicadorBalanceSocialRepository repository, ILogger<BalanceSocialService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<IndicadorBalanceSocialDto?> GetIndicadorAsync(Guid asociadoId, int anio)
    {
        _logger.LogInformation(
            "Obteniendo indicador de balance social para asociado {AsociadoId}, año {Anio}",
            asociadoId, anio);

        var indicador = await _repository.GetByAsociadoAndAnioAsync(asociadoId, anio);
        return indicador != null ? MapToDto(indicador) : null;
    }

    /// <inheritdoc/>
    public async Task<IndicadorBalanceSocialDto> CalcularIndicadorAsync(Guid asociadoId, Guid organizationId, int anio)
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

        var existing = await _repository.GetByAsociadoAndAnioAsync(asociadoId, anio);

        var indicador = new IndicadorBalanceSocial
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
            IndiceBalanceSocial = indiceCompuesto,
            CreatedAt = existing?.CreatedAt ?? DateTime.UtcNow
        };

        if (existing != null)
        {
            var updated = await _repository.UpdateAsync(indicador);
            return MapToDto(updated);
        }

        var creado = await _repository.CreateAsync(indicador);
        return MapToDto(creado);
    }

    /// <inheritdoc/>
    public async Task<List<IndicadorBalanceSocialDto>> GetIndicadoresByOrganizacionAsync(Guid organizationId, int anio)
    {
        _logger.LogInformation(
            "Obteniendo indicadores de balance social de organización {OrgId}, año {Anio}",
            organizationId, anio);

        var indicadores = await _repository.GetByOrganizationAndAnioAsync(organizationId, anio);
        return indicadores.Select(MapToDto).ToList();
    }

    /// <inheritdoc/>
    public async Task<List<IndicadorBalanceSocialDto>> GetNoCumplenEducacionAsync(Guid organizationId, int anio)
    {
        _logger.LogInformation(
            "Obteniendo asociados que NO cumplen educación mínima en organización {OrgId}, año {Anio}",
            organizationId, anio);

        var indicadores = await _repository.GetByOrganizationAndAnioAsync(organizationId, anio);
        var result = indicadores
            .Where(i => !i.CumpleEducacion)
            .ToList();

        return result.Select(MapToDto).ToList();
    }

    private static IndicadorBalanceSocialDto MapToDto(IndicadorBalanceSocial i) => new()
    {
        Id = i.Id,
        AsociadoId = i.AsociadoId,
        OrganizationId = i.OrganizationId,
        Anio = i.Anio,
        HorasEducacion = i.HorasEducacion,
        ParticipacionAsambleas = i.ParticipacionAsambleas,
        ParticipacionComites = i.ParticipacionComites,
        AportesSociales = i.AportesSociales,
        BeneficiosRecibidos = i.BeneficiosRecibidos,
        CumpleEducacion = i.CumpleEducacion,
        IndiceBalanceSocial = i.IndiceBalanceSocial,
        Observaciones = i.Observaciones
    };
}