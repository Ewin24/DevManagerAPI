namespace Application.Tests.Services;

using Application.DTOs.Nomina;
using Application.Interfaces;
using Application.Services.Nomina;
using Domain.Enums;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

/// <summary>
/// Tests unitarios para CompensacionService (Fase 1 — NominaAsociados)
/// </summary>
public class CompensacionServiceTests
{
    private readonly Mock<ILogger<CompensacionService>> _loggerMock;
    private readonly ICompensacionService _compensacionService;

    public CompensacionServiceTests()
    {
        _loggerMock = new Mock<ILogger<CompensacionService>>();
        _compensacionService = new CompensacionService(_loggerMock.Object);
    }

    [Fact]
    public async Task CreateAsync_WithDiasPorTarifa_CalculatesCorrectly()
    {
        // Arrange
        var dto = new CreateCompensacionDto
        {
            AsociadoId = Guid.NewGuid(),
            OrganizationId = Guid.NewGuid(),
            Periodo = new DateTime(2026, 7, 1),
            Modelo = CompensacionModelo.DiasPorTarifa,
            ValorBase = 20m // 20 días
        };

        // Act
        var result = await _compensacionService.CreateAsync(dto);

        // Assert: 20 días × 50,000 = 1,000,000
        result.Should().NotBeNull();
        result.Modelo.Should().Be(CompensacionModelo.DiasPorTarifa);
        result.ValorBase.Should().Be(20m);
        result.ValorCalculado.Should().Be(1_000_000m);
        result.AsociadoId.Should().Be(dto.AsociadoId);
    }

    [Fact]
    public async Task CreateAsync_WithFijoMensual_ReturnsFixedAmount()
    {
        // Arrange
        var dto = new CreateCompensacionDto
        {
            AsociadoId = Guid.NewGuid(),
            OrganizationId = Guid.NewGuid(),
            Periodo = new DateTime(2026, 7, 1),
            Modelo = CompensacionModelo.FijoMensual,
            ValorBase = 2_500_000m // Monto fijo mensual
        };

        // Act
        var result = await _compensacionService.CreateAsync(dto);

        // Assert: retorna el valor base como monto fijo
        result.Should().NotBeNull();
        result.Modelo.Should().Be(CompensacionModelo.FijoMensual);
        result.ValorBase.Should().Be(2_500_000m);
        result.ValorCalculado.Should().Be(2_500_000m);
    }

    [Fact]
    public async Task CreateAsync_WithPorProyecto_ReturnsProjectAmount()
    {
        // Arrange
        var dto = new CreateCompensacionDto
        {
            AsociadoId = Guid.NewGuid(),
            OrganizationId = Guid.NewGuid(),
            Periodo = new DateTime(2026, 7, 1),
            Modelo = CompensacionModelo.PorProyecto,
            ValorBase = 3_200_000m // Monto del proyecto
        };

        // Act
        var result = await _compensacionService.CreateAsync(dto);

        // Assert: retorna el valor base como monto del proyecto
        result.Should().NotBeNull();
        result.Modelo.Should().Be(CompensacionModelo.PorProyecto);
        result.ValorBase.Should().Be(3_200_000m);
        result.ValorCalculado.Should().Be(3_200_000m);
    }
}
