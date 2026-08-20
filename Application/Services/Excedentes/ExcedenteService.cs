namespace Application.Services.Excedentes;

using Application.DTOs.Excedentes;
using Application.Interfaces;
using Domain.Entities.Excedentes;
using Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

/// <summary>
/// Implementación del servicio de distribución de excedentes
/// Ley 79 art. 54: 20% Reserva, 20% Educación, 10% Solidaridad
/// El remanente (50%) se distribuye según decisión de Asamblea General
/// </summary>
public class ExcedenteService : IExcedenteService
{
    private readonly IExcedenteRepository _repository;
    private readonly ILogger<ExcedenteService> _logger;

    private const decimal PorcentajeReserva = 0.20m;
    private const decimal PorcentajeEducacion = 0.20m;
    private const decimal PorcentajeSolidaridad = 0.10m;

    public ExcedenteService(IExcedenteRepository repository, ILogger<ExcedenteService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<ExcedenteDto> CalcularDistribucionAsync(CreateExcedenteDto dto)
    {
        _logger.LogInformation(
            "Calculando distribución de excedentes para org {OrgId}, período {Periodo}, total {Total:N2}",
            dto.OrganizationId, dto.Periodo, dto.TotalExcedentes);

        if (dto.TotalExcedentes <= 0)
        {
            throw new InvalidOperationException(
                "No hay excedentes para distribuir. El total debe ser positivo.");
        }

        var reserva = Math.Round(dto.TotalExcedentes * PorcentajeReserva, 2, MidpointRounding.AwayFromZero);
        var educacion = Math.Round(dto.TotalExcedentes * PorcentajeEducacion, 2, MidpointRounding.AwayFromZero);
        var solidaridad = Math.Round(dto.TotalExcedentes * PorcentajeSolidaridad, 2, MidpointRounding.AwayFromZero);

        // Verificar si ya existe distribución para el mismo período
        var existing = await _repository.GetByOrganizationAndPeriodoAsync(dto.OrganizationId, dto.Periodo);

        if (existing != null)
        {
            existing.TotalExcedentes = dto.TotalExcedentes;
            existing.ReservaProteccionAportes = reserva;
            existing.FondoEducacion = educacion;
            existing.FondoSolidaridad = solidaridad;
            existing.Observaciones = dto.Observaciones ?? existing.Observaciones;

            var updated = await _repository.UpdateAsync(existing);
            return MapToDto(updated);
        }

        var excedente = new Excedente
        {
            Id = Guid.NewGuid(),
            OrganizationId = dto.OrganizationId,
            Periodo = dto.Periodo,
            TotalExcedentes = dto.TotalExcedentes,
            ReservaProteccionAportes = reserva,
            FondoEducacion = educacion,
            FondoSolidaridad = solidaridad,
            AprobadoPorAsamblea = false,
            Observaciones = dto.Observaciones,
            CreatedAt = DateTime.UtcNow
        };

        var creado = await _repository.CreateAsync(excedente);
        return MapToDto(creado);
    }

    /// <inheritdoc/>
    public async Task<ExcedenteDto?> GetByPeriodoAsync(Guid organizationId, DateTime periodo)
    {
        var result = await _repository.GetByOrganizationAndPeriodoAsync(organizationId, periodo);
        return result != null ? MapToDto(result) : null;
    }

    /// <inheritdoc/>
    public async Task<List<ExcedenteDto>> GetByOrganizacionAsync(Guid organizationId)
    {
        var result = await _repository.GetByOrganizationAsync(organizationId);
        return result.Select(MapToDto).ToList();
    }

    /// <inheritdoc/>
    public async Task<ExcedenteDto> AprobarDistribucionAsync(
        Guid excedenteId, decimal? revalorizacion, decimal? retornoCooperativo)
    {
        _logger.LogInformation(
            "Aprobando distribución de excedentes {ExcedenteId} en Asamblea",
            excedenteId);

        var existing = await _repository.GetByIdAsync(excedenteId);
        if (existing == null)
            throw new KeyNotFoundException($"Excedente {excedenteId} no encontrado");

        // El remanente después de 20/20/10 es el 50%
        var remanente = existing.TotalExcedentes -
            existing.ReservaProteccionAportes -
            existing.FondoEducacion -
            existing.FondoSolidaridad;

        // Validar que revalorización + retorno no excedan el remanente
        var totalDistribuir = (revalorizacion ?? 0) + (retornoCooperativo ?? 0);
        if (totalDistribuir > remanente)
        {
            throw new InvalidOperationException(
                $"La suma de revalorización ({revalorizacion:N2}) y retorno ({retornoCooperativo:N2}) " +
                $"excede el remanente disponible ({remanente:N2})");
        }

        existing.Revalorizacion = revalorizacion;
        existing.RetornoCooperativo = retornoCooperativo;
        existing.AprobadoPorAsamblea = true;

        var updated = await _repository.UpdateAsync(existing);
        return MapToDto(updated);
    }

    private static ExcedenteDto MapToDto(Excedente e) => new()
    {
        Id = e.Id,
        OrganizationId = e.OrganizationId,
        Periodo = e.Periodo,
        TotalExcedentes = e.TotalExcedentes,
        ReservaProteccionAportes = e.ReservaProteccionAportes,
        FondoEducacion = e.FondoEducacion,
        FondoSolidaridad = e.FondoSolidaridad,
        Revalorizacion = e.Revalorizacion,
        RetornoCooperativo = e.RetornoCooperativo,
        AprobadoPorAsamblea = e.AprobadoPorAsamblea,
        Observaciones = e.Observaciones,
        CreatedAt = e.CreatedAt
    };
}