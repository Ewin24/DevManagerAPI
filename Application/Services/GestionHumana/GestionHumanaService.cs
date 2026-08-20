namespace Application.Services.GestionHumana;

using Application.DTOs.GestionHumana;
using Application.Interfaces;
using Domain.Entities.GestionHumana;
using Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

/// <summary>
/// Implementación del servicio de gestión humana solidaria
/// Administra competencias cooperativas y perfiles con dimensión solidaria
/// </summary>
public class GestionHumanaService : IGestionHumanaService
{
    private readonly ICompetenciaAsociadoRepository _repository;
    private readonly ILogger<GestionHumanaService> _logger;

    public GestionHumanaService(ICompetenciaAsociadoRepository repository, ILogger<GestionHumanaService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<List<CompetenciaAsociadoDto>> GetCompetenciasAsync(Guid asociadoId)
    {
        _logger.LogInformation("Obteniendo competencias del asociado {AsociadoId}", asociadoId);

        var competencias = await _repository.GetByAsociadoAsync(asociadoId);
        return competencias.Select(MapToDto).ToList();
    }

    /// <inheritdoc/>
    public async Task<CompetenciaAsociadoDto> CreateCompetenciaAsync(Guid asociadoId, Guid organizationId, string competencia, int nivel)
    {
        _logger.LogInformation(
            "Registrando competencia '{Competencia}' nivel {Nivel} para asociado {AsociadoId}",
            competencia, nivel, asociadoId);

        var entity = new CompetenciaAsociado
        {
            Id = Guid.NewGuid(),
            AsociadoId = asociadoId,
            OrganizationId = organizationId,
            Competencia = competencia,
            Nivel = Math.Clamp(nivel, 1, 5),
            Disponible = true,
            FechaActualizacion = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        var creada = await _repository.CreateAsync(entity);
        return MapToDto(creada);
    }

    /// <inheritdoc/>
    public async Task<CompetenciaAsociadoDto> UpdateDisponibilidadAsync(Guid competenciaId, bool disponible)
    {
        _logger.LogInformation(
            "Actualizando disponibilidad de competencia {CompetenciaId} a {Disponible}",
            competenciaId, disponible);

        var existing = await _repository.GetByIdAsync(competenciaId);
        if (existing == null)
        {
            throw new KeyNotFoundException($"Competencia {competenciaId} no encontrada");
        }

        existing.Disponible = disponible;

        var updated = await _repository.UpdateAsync(existing);
        return MapToDto(updated);
    }

    /// <inheritdoc/>
    public async Task<List<CompetenciaAsociadoDto>> BuscarPorCompetenciaAsync(string competencia, bool soloDisponibles = true)
    {
        _logger.LogInformation(
            "Buscando asociados por competencia '{Competencia}', solo disponibles: {SoloDisponibles}",
            competencia, soloDisponibles);

        var resultados = await _repository.SearchByCompetenciaAsync(competencia, soloDisponibles);
        return resultados.Select(MapToDto).ToList();
    }

    private static CompetenciaAsociadoDto MapToDto(CompetenciaAsociado c) => new()
    {
        Id = c.Id,
        AsociadoId = c.AsociadoId,
        Competencia = c.Competencia,
        Nivel = c.Nivel,
        Disponible = c.Disponible,
        FechaActualizacion = c.FechaActualizacion,
        Observaciones = c.Observaciones
    };
}