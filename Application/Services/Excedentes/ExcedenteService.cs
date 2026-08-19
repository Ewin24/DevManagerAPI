namespace Application.Services.Excedentes;

using Application.DTOs.Excedentes;
using Application.Interfaces;
using Microsoft.Extensions.Logging;

/// <summary>
/// Implementación del servicio de distribución de excedentes
/// Ley 79 art. 54: 20% Reserva, 20% Educación, 10% Solidaridad
/// El remanente (50%) se distribuye según decisión de Asamblea General
/// </summary>
public class ExcedenteService : IExcedenteService
{
    private readonly ILogger<ExcedenteService> _logger;
    private readonly List<ExcedenteDto> _excedentesStore = new();

    private const decimal PorcentajeReserva = 0.20m;
    private const decimal PorcentajeEducacion = 0.20m;
    private const decimal PorcentajeSolidaridad = 0.10m;

    public ExcedenteService(ILogger<ExcedenteService> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc/>
    public Task<ExcedenteDto> CalcularDistribucionAsync(CreateExcedenteDto dto)
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
        var existing = _excedentesStore.FirstOrDefault(e =>
            e.OrganizationId == dto.OrganizationId && e.Periodo == dto.Periodo);

        if (existing != null)
        {
            var updated = existing with
            {
                TotalExcedentes = dto.TotalExcedentes,
                ReservaProteccionAportes = reserva,
                FondoEducacion = educacion,
                FondoSolidaridad = solidaridad,
                Observaciones = dto.Observaciones ?? existing.Observaciones
            };

            var index = _excedentesStore.IndexOf(existing);
            _excedentesStore[index] = updated;
            return Task.FromResult(updated);
        }

        var excedente = new ExcedenteDto
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

        _excedentesStore.Add(excedente);
        return Task.FromResult(excedente);
    }

    /// <inheritdoc/>
    public Task<ExcedenteDto?> GetByPeriodoAsync(Guid organizationId, DateTime periodo)
    {
        var result = _excedentesStore.FirstOrDefault(e =>
            e.OrganizationId == organizationId && e.Periodo == periodo);

        return Task.FromResult(result);
    }

    /// <inheritdoc/>
    public Task<List<ExcedenteDto>> GetByOrganizacionAsync(Guid organizationId)
    {
        var result = _excedentesStore
            .Where(e => e.OrganizationId == organizationId)
            .OrderByDescending(e => e.Periodo)
            .ToList();

        return Task.FromResult(result);
    }

    /// <inheritdoc/>
    public Task<ExcedenteDto> AprobarDistribucionAsync(
        Guid excedenteId, decimal? revalorizacion, decimal? retornoCooperativo)
    {
        _logger.LogInformation(
            "Aprobando distribución de excedentes {ExcedenteId} en Asamblea",
            excedenteId);

        var existing = _excedentesStore.FirstOrDefault(e => e.Id == excedenteId);
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

        var updated = existing with
        {
            Revalorizacion = revalorizacion,
            RetornoCooperativo = retornoCooperativo,
            AprobadoPorAsamblea = true
        };

        var index = _excedentesStore.IndexOf(existing);
        _excedentesStore[index] = updated;

        return Task.FromResult(updated);
    }
}
