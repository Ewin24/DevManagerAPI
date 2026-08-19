namespace Application.Tests.Services;

using Application.Interfaces;
using Application.Services.BalanceSocial;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

/// <summary>
/// Tests unitarios para BalanceSocialService (Fase 2 — Indicadores de gestión social)
/// </summary>
public class BalanceSocialServiceTests
{
    private readonly Mock<ILogger<BalanceSocialService>> _loggerMock;
    private readonly IBalanceSocialService _service;

    public BalanceSocialServiceTests()
    {
        _loggerMock = new Mock<ILogger<BalanceSocialService>>();
        _service = new BalanceSocialService(_loggerMock.Object);
    }

    [Fact]
    public async Task CalcularIndicador_WithNewAsociado_CreatesIndicator()
    {
        // Arrange
        var asociadoId = Guid.NewGuid();
        var orgId = Guid.NewGuid();

        // Act
        var result = await _service.CalcularIndicadorAsync(asociadoId, orgId, 2026);

        // Assert
        result.Should().NotBeNull();
        result.AsociadoId.Should().Be(asociadoId);
        result.Anio.Should().Be(2026);
        result.HorasEducacion.Should().Be(0);
        result.CumpleEducacion.Should().BeFalse();
    }

    [Fact]
    public async Task GetIndicador_AfterCalculation_ReturnsStoredValue()
    {
        // Arrange
        var asociadoId = Guid.NewGuid();
        var orgId = Guid.NewGuid();

        await _service.CalcularIndicadorAsync(asociadoId, orgId, 2026);

        // Act
        var result = await _service.GetIndicadorAsync(asociadoId, 2026);

        // Assert
        result.Should().NotBeNull();
        result!.Anio.Should().Be(2026);
    }

    [Fact]
    public async Task GetNoCumplenEducacion_WithNoEducation_ReturnsInList()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var asociado1 = Guid.NewGuid();
        var asociado2 = Guid.NewGuid();

        await _service.CalcularIndicadorAsync(asociado1, orgId, 2026);
        await _service.CalcularIndicadorAsync(asociado2, orgId, 2026);

        // Act
        var result = await _service.GetNoCumplenEducacionAsync(orgId, 2026);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(2); // Ambos con 0 horas
    }
}
