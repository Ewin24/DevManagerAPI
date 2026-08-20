namespace Application.Tests.Services;

using Application.DTOs.Excedentes;
using Application.Interfaces;
using Application.Services.Excedentes;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

/// <summary>
/// Tests unitarios para ExcedenteService (Fase 3 — Ley 79 art. 54)
/// Distribución 20/20/10
/// </summary>
public class ExcedenteServiceTests
{
    private readonly Mock<ILogger<ExcedenteService>> _loggerMock;
    private readonly IExcedenteService _service;

    public ExcedenteServiceTests()
    {
        _loggerMock = new Mock<ILogger<ExcedenteService>>();
        _service = new ExcedenteService(
            new InMemoryExcedenteRepository(new InMemoryStores()),
            _loggerMock.Object);
    }

    [Fact]
    public async Task CalcularDistribucion_With100MExcedentes_Calculates202010Correctly()
    {
        // Arrange
        var dto = new CreateExcedenteDto
        {
            OrganizationId = Guid.NewGuid(),
            Periodo = new DateTime(2026, 12, 31),
            TotalExcedentes = 100_000_000m // $100M COP
        };

        // Act
        var result = await _service.CalcularDistribucionAsync(dto);

        // Assert: Ley 79 art. 54 — 20% reserva, 20% educación, 10% solidaridad
        result.Should().NotBeNull();
        result.TotalExcedentes.Should().Be(100_000_000m);
        result.ReservaProteccionAportes.Should().Be(20_000_000m); // 20%
        result.FondoEducacion.Should().Be(20_000_000m); // 20%
        result.FondoSolidaridad.Should().Be(10_000_000m); // 10%
        result.AprobadoPorAsamblea.Should().BeFalse();
    }

    [Fact]
    public async Task CalcularDistribucion_WithZeroExcedentes_ThrowsInvalidOperation()
    {
        // Arrange
        var dto = new CreateExcedenteDto
        {
            OrganizationId = Guid.NewGuid(),
            Periodo = new DateTime(2026, 6, 30),
            TotalExcedentes = 0
        };

        // Act
        Func<Task> act = () => _service.CalcularDistribucionAsync(dto);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*excedentes*positivo*");
    }

    [Fact]
    public async Task AprobarDistribucion_WithValidRemanente_MarksAsAprobado()
    {
        // Arrange
        var excedente = await _service.CalcularDistribucionAsync(new CreateExcedenteDto
        {
            OrganizationId = Guid.NewGuid(),
            Periodo = new DateTime(2026, 12, 31),
            TotalExcedentes = 100_000_000m // $100M
        });

        // Act — remanente después de 20/20/10 = $50M, aprobar con $30M revalorización + $20M retorno
        var result = await _service.AprobarDistribucionAsync(excedente.Id, 30_000_000m, 20_000_000m);

        // Assert
        result.AprobadoPorAsamblea.Should().BeTrue();
        result.Revalorizacion.Should().Be(30_000_000m);
        result.RetornoCooperativo.Should().Be(20_000_000m);
    }

    [Fact]
    public async Task AprobarDistribucion_WithExcesiveRemanente_ThrowsInvalidOperation()
    {
        // Arrange
        var excedente = await _service.CalcularDistribucionAsync(new CreateExcedenteDto
        {
            OrganizationId = Guid.NewGuid(),
            Periodo = new DateTime(2026, 12, 31),
            TotalExcedentes = 100_000_000m // remanente = $50M
        });

        // Act — intentar distribuir $60M cuando el remanente es $50M
        Func<Task> act = () => _service.AprobarDistribucionAsync(excedente.Id, 40_000_000m, 20_000_000m);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*excede*remanente*");
    }
}
