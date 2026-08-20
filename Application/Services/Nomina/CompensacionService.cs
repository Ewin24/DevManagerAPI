namespace Application.Services.Nomina;

using Application.DTOs.Nomina;
using Application.Interfaces;
using Domain.Entities.Nomina;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

/// <summary>
/// Implementación del servicio de compensación para asociados
/// Soporta 3 modelos: días×tarifa, fijo mensual, por proyecto
/// </summary>
public class CompensacionService : ICompensacionService
{
    private readonly ICompensacionRepository _repository;
    private readonly ILogger<CompensacionService> _logger;

    public CompensacionService(ICompensacionRepository repository, ILogger<CompensacionService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<CompensacionDto> CreateAsync(CreateCompensacionDto dto)
    {
        _logger.LogInformation(
            "Creando compensación para asociado {AsociadoId}, modelo {Modelo}",
            dto.AsociadoId, dto.Modelo);

        var calculado = await CalcularByModeloAsync(dto.Modelo, dto.ValorBase);

        var compensacion = new Compensacion
        {
            Id = Guid.NewGuid(),
            AsociadoId = dto.AsociadoId,
            OrganizationId = dto.OrganizationId,
            Periodo = dto.Periodo,
            Modelo = dto.Modelo,
            ValorBase = dto.ValorBase,
            ValorCalculado = calculado,
            Observaciones = dto.Observaciones,
            CreatedAt = DateTime.UtcNow
        };

        var creada = await _repository.CreateAsync(compensacion);
        return MapToDto(creada);
    }

    /// <inheritdoc/>
    public Task<decimal> CalcularAsync(Guid asociadoId, int mes, int anio)
    {
        // Cálculo base: en una implementación completa se obtendrían
        // los datos del asociado y los parámetros de compensación
        _logger.LogInformation("Calculando compensación para asociado {AsociadoId}, {Mes}/{Anio}",
            asociadoId, mes, anio);

        var periodo = new DateTime(anio, mes, 1);
        var modelo = CompensacionModelo.DiasPorTarifa;
        var valorBase = 20m; // 20 días por defecto

        return CalcularByModeloAsync(modelo, valorBase);
    }

    /// <inheritdoc/>
    public async Task<List<CompensacionDto>> GetByAsociadoAsync(Guid asociadoId, int anio)
    {
        _logger.LogInformation("Obteniendo compensaciones de asociado {AsociadoId}, año {Anio}",
            asociadoId, anio);

        var compensaciones = await _repository.GetByAsociadoAsync(asociadoId, anio);
        return compensaciones.Select(MapToDto).ToList();
    }

    /// <summary>
    /// Calcula el valor de compensación según el modelo
    /// </summary>
    /// <param name="modelo">Modelo de cálculo</param>
    /// <param name="valorBase">Valor base (días o monto fijo)</param>
    private Task<decimal> CalcularByModeloAsync(CompensacionModelo modelo, decimal valorBase)
    {
        const decimal tarifaDiaria = 50000m; // Tarifa por defecto

        var resultado = modelo switch
        {
            CompensacionModelo.DiasPorTarifa => valorBase * tarifaDiaria,
            CompensacionModelo.FijoMensual => valorBase,
            CompensacionModelo.PorProyecto => valorBase,
            _ => throw new ArgumentOutOfRangeException(nameof(modelo), modelo, "Modelo de compensación no válido")
        };

        return Task.FromResult(resultado);
    }

    private static CompensacionDto MapToDto(Compensacion c) => new()
    {
        Id = c.Id,
        AsociadoId = c.AsociadoId,
        OrganizationId = c.OrganizationId,
        Periodo = c.Periodo,
        Modelo = c.Modelo,
        ValorBase = c.ValorBase,
        ValorCalculado = c.ValorCalculado,
        Observaciones = c.Observaciones,
        CreatedAt = c.CreatedAt
    };
}