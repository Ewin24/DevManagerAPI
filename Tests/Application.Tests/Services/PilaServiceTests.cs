namespace Application.Tests.Services;

using Application.Interfaces;
using Application.Services.Nomina;
using Domain.Enums;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

/// <summary>
/// Tests unitarios para PilaService (Fase 1 — NominaAsociados)
/// Validan tasas según Decreto 2150/2017
/// </summary>
public class PilaServiceTests
{
    private readonly Mock<ILogger<PilaService>> _loggerMock;
    private readonly IPilaService _pilaService;

    public PilaServiceTests()
    {
        _loggerMock = new Mock<ILogger<PilaService>>();
        _pilaService = new PilaService(_loggerMock.Object);
    }

    [Fact]
    public async Task CalcularAportesAsync_WithRiesgo1_AppliesCorrectRates()
    {
        // Arrange
        var asociadoId = Guid.NewGuid();
        var ingresos = 2_000_000m;
        var nivelRiesgo = 1;

        // EPS 12.5% = 250,000 | Pensión 16% = 320,000 | ARL 0.522% = 10,440
        // Total = 580,440
        var expectedEPS = 250_000m;
        var expectedPension = 320_000m;
        var expectedARL = 10_440m;
        var expectedTotal = 580_440m;

        // Act
        var result = await _pilaService.CalcularAportesAsync(asociadoId, ingresos, nivelRiesgo);

        // Assert
        result.Should().NotBeNull();
        result.TipoAportante.Should().Be(PilaTipoAportante.Independiente);
        result.IngresoBase.Should().Be(ingresos);
        result.AporteEPS.Should().Be(expectedEPS);
        result.AportePension.Should().Be(expectedPension);
        result.AporteARL.Should().Be(expectedARL);
        result.Total.Should().Be(expectedTotal);
    }

    [Fact]
    public async Task CalcularAportesAsync_WithRiesgo5_AppliesHigherARL()
    {
        // Arrange
        var asociadoId = Guid.NewGuid();
        var ingresos = 2_000_000m;
        var nivelRiesgo = 5;

        // EPS 12.5% = 250,000 | Pensión 16% = 320,000 | ARL 6.96% = 139,200
        // Total = 709,200
        var expectedEPS = 250_000m;
        var expectedPension = 320_000m;
        var expectedARL = 139_200m;
        var expectedTotal = 709_200m;

        // Act
        var result = await _pilaService.CalcularAportesAsync(asociadoId, ingresos, nivelRiesgo);

        // Assert
        result.Should().NotBeNull();
        result.TipoAportante.Should().Be(PilaTipoAportante.Independiente);
        result.IngresoBase.Should().Be(ingresos);
        result.AporteEPS.Should().Be(expectedEPS);
        result.AportePension.Should().Be(expectedPension);
        result.AporteARL.Should().Be(expectedARL);
        result.Total.Should().Be(expectedTotal);
    }
}
