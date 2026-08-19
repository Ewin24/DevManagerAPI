namespace Application.Services.GestionHumana;

using Application.DTOs.GestionHumana;
using Application.Interfaces;
using Microsoft.Extensions.Logging;

/// <summary>
/// Implementación del servicio de gestión humana solidaria
/// Administra competencias cooperativas y perfiles con dimensión solidaria
/// </summary>
public class GestionHumanaService : IGestionHumanaService
{
    private readonly ILogger<GestionHumanaService> _logger;
    private readonly List<CompetenciaAsociadoDto> _competenciasStore; // Simulación en memoria

    public GestionHumanaService(ILogger<GestionHumanaService> logger)
    {
        _logger = logger;
        _competenciasStore = new List<CompetenciaAsociadoDto>();
    }

    /// <inheritdoc/>
    public Task<List<CompetenciaAsociadoDto>> GetCompetenciasAsync(Guid asociadoId)
    {
        _logger.LogInformation("Obteniendo competencias del asociado {AsociadoId}", asociadoId);

        var result = _competenciasStore
            .Where(c => c.AsociadoId == asociadoId)
            .ToList();

        return Task.FromResult(result);
    }

    /// <inheritdoc/>
    public Task<CompetenciaAsociadoDto> CreateCompetenciaAsync(Guid asociadoId, Guid organizationId, string competencia, int nivel)
    {
        _logger.LogInformation(
            "Registrando competencia '{Competencia}' nivel {Nivel} para asociado {AsociadoId}",
            competencia, nivel, asociadoId);

        var dto = new CompetenciaAsociadoDto
        {
            Id = Guid.NewGuid(),
            AsociadoId = asociadoId,
            Competencia = competencia,
            Nivel = Math.Clamp(nivel, 1, 5),
            Disponible = true,
            FechaActualizacion = DateTime.UtcNow
        };

        _competenciasStore.Add(dto);
        return Task.FromResult(dto);
    }

    /// <inheritdoc/>
    public Task<CompetenciaAsociadoDto> UpdateDisponibilidadAsync(Guid competenciaId, bool disponible)
    {
        _logger.LogInformation(
            "Actualizando disponibilidad de competencia {CompetenciaId} a {Disponible}",
            competenciaId, disponible);

        var existing = _competenciasStore.FirstOrDefault(c => c.Id == competenciaId);
        if (existing == null)
        {
            throw new KeyNotFoundException($"Competencia {competenciaId} no encontrada");
        }

        var updated = existing with { Disponible = disponible };
        var index = _competenciasStore.IndexOf(existing);
        _competenciasStore[index] = updated;

        return Task.FromResult(updated);
    }

    /// <inheritdoc/>
    public Task<List<CompetenciaAsociadoDto>> BuscarPorCompetenciaAsync(string competencia, bool soloDisponibles = true)
    {
        _logger.LogInformation(
            "Buscando asociados por competencia '{Competencia}', solo disponibles: {SoloDisponibles}",
            competencia, soloDisponibles);

        var query = _competenciasStore
            .Where(c => c.Competencia.Contains(competencia, StringComparison.OrdinalIgnoreCase));

        if (soloDisponibles)
        {
            query = query.Where(c => c.Disponible);
        }

        return Task.FromResult(query.ToList());
    }
}
