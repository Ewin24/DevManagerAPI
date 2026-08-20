namespace Application.Tests.Services;

using Application.DTOs.GestionHumana;
using Application.Interfaces;
using Application.Services.GestionHumana;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

/// <summary>
/// Tests unitarios para GestionHumanaService (Fase 2 — Competencias cooperativas)
/// </summary>
public class GestionHumanaServiceTests
{
    private readonly Mock<ILogger<GestionHumanaService>> _loggerMock;
    private readonly IGestionHumanaService _service;

    public GestionHumanaServiceTests()
    {
        _loggerMock = new Mock<ILogger<GestionHumanaService>>();
        _service = new GestionHumanaService(
            new InMemoryCompetenciaAsociadoRepository(new InMemoryStores()),
            _loggerMock.Object);
    }

    [Fact]
    public async Task CreateCompetencia_WithValidData_ReturnsCompetencia()
    {
        // Arrange
        var asociadoId = Guid.NewGuid();
        var orgId = Guid.NewGuid();

        // Act
        var result = await _service.CreateCompetenciaAsync(
            asociadoId, orgId, "Facilitación de asambleas", 4);

        // Assert
        result.Should().NotBeNull();
        result.AsociadoId.Should().Be(asociadoId);
        result.Competencia.Should().Be("Facilitación de asambleas");
        result.Nivel.Should().Be(4);
        result.Disponible.Should().BeTrue();
    }

    [Fact]
    public async Task GetCompetencias_WithExistingCompetencias_ReturnsList()
    {
        // Arrange
        var asociadoId = Guid.NewGuid();
        var orgId = Guid.NewGuid();

        await _service.CreateCompetenciaAsync(asociadoId, orgId, "Contabilidad solidaria", 3);
        await _service.CreateCompetenciaAsync(asociadoId, orgId, "Gobierno cooperativo", 5);

        // Act
        var result = await _service.GetCompetenciasAsync(asociadoId);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(2);
    }

    [Fact]
    public async Task UpdateDisponibilidad_WithExistingCompetencia_UpdatesCorrectly()
    {
        // Arrange
        var asociadoId = Guid.NewGuid();
        var orgId = Guid.NewGuid();

        var competencia = await _service.CreateCompetenciaAsync(
            asociadoId, orgId, "Auditoría solidaria", 2);

        // Act
        var result = await _service.UpdateDisponibilidadAsync(competencia.Id, false);

        // Assert
        result.Disponible.Should().BeFalse();
    }

    [Fact]
    public async Task BuscarPorCompetencia_WithMatchingName_ReturnsResults()
    {
        // Arrange
        var asociadoId = Guid.NewGuid();
        var orgId = Guid.NewGuid();

        await _service.CreateCompetenciaAsync(asociadoId, orgId, "Facilitación", 4);
        await _service.CreateCompetenciaAsync(
            Guid.NewGuid(), orgId, "Contabilidad solidaria", 3);
        await _service.CreateCompetenciaAsync(asociadoId, orgId, "Facilitación avanzada", 5);

        // Act
        var result = await _service.BuscarPorCompetenciaAsync("Facilitación");

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(2);
    }
}
