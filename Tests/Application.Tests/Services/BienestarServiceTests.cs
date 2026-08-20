namespace Application.Tests.Services;

using Application.DTOs.Bienestar;
using Application.Interfaces;
using Application.Services.Bienestar;
using Domain.Enums;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

/// <summary>
/// Tests unitarios para BienestarService (Fase 2 — Auxilios, becas, fondo solidaridad)
/// </summary>
public class BienestarServiceTests
{
    private readonly Mock<ILogger<BienestarService>> _loggerMock;
    private readonly IBienestarService _service;

    public BienestarServiceTests()
    {
        _loggerMock = new Mock<ILogger<BienestarService>>();
        var stores = new InMemoryStores();
        _service = new BienestarService(
            new InMemoryProgramaBienestarRepository(stores),
            new InMemorySolicitudBienestarRepository(stores),
            new InMemoryAuxilioRepository(stores),
            new InMemoryFondoSolidaridadRepository(stores),
            _loggerMock.Object);
    }

    [Fact]
    public async Task CreateSolicitud_WithValidData_ReturnsPendiente()
    {
        // Arrange
        var dto = new CreateSolicitudBienestarDto
        {
            AsociadoId = Guid.NewGuid(),
            OrganizationId = Guid.NewGuid(),
            TipoAuxilio = TipoAuxilio.AuxilioEconomico,
            MontoSolicitado = 500_000m,
            Motivo = "Calamidad doméstica",
            FechaRequerida = DateTime.UtcNow.AddDays(7)
        };

        // Act
        var result = await _service.CreateSolicitudAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Estado.Should().Be(EstadoSolicitudBienestar.Pendiente);
        result.TipoAuxilio.Should().Be(TipoAuxilio.AuxilioEconomico);
        result.MontoSolicitado.Should().Be(500_000m);
    }

    [Fact]
    public async Task AprobarSolicitud_WithValidId_UpdatesEstado()
    {
        // Arrange
        var solicitud = await _service.CreateSolicitudAsync(new CreateSolicitudBienestarDto
        {
            AsociadoId = Guid.NewGuid(),
            OrganizationId = Guid.NewGuid(),
            TipoAuxilio = TipoAuxilio.BecaEducativa,
            MontoSolicitado = 2_000_000m,
            Motivo = "Beca para hijo en universidad",
            FechaRequerida = DateTime.UtcNow.AddMonths(1)
        });

        // Act
        var result = await _service.AprobarSolicitudAsync(
            solicitud.Id, 1_500_000m, Guid.NewGuid());

        // Assert
        result.Estado.Should().Be(EstadoSolicitudBienestar.Aprobada);
        result.MontoAprobado.Should().Be(1_500_000m);
        result.FechaResolucion.Should().NotBeNull();
    }

    [Fact]
    public async Task RechazarSolicitud_WithValidId_UpdatesEstado()
    {
        // Arrange
        var solicitud = await _service.CreateSolicitudAsync(new CreateSolicitudBienestarDto
        {
            AsociadoId = Guid.NewGuid(),
            OrganizationId = Guid.NewGuid(),
            TipoAuxilio = TipoAuxilio.CreditoBlando,
            MontoSolicitado = 5_000_000m,
            Motivo = "Crédito para equipo médico",
            FechaRequerida = DateTime.UtcNow.AddDays(15)
        });

        // Act
        var result = await _service.RechazarSolicitudAsync(
            solicitud.Id, "No cumple requisitos de permanencia", Guid.NewGuid());

        // Assert
        result.Estado.Should().Be(EstadoSolicitudBienestar.Rechazada);
        result.ObservacionesResolucion.Should().Be("No cumple requisitos de permanencia");
    }

    [Fact]
    public async Task CalcularAporteFondo_WithExcedentes_Calculates10Percent()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var periodo = new DateTime(2026, 6, 1);
        var totalExcedentes = 100_000_000m; // $100M COP

        // Act
        var result = await _service.CalcularAporteFondoAsync(orgId, periodo, totalExcedentes);

        // Assert: 10% Ley 79 art.54
        result.Should().NotBeNull();
        result.TotalExcedentes.Should().Be(100_000_000m);
        result.AporteFondo.Should().Be(10_000_000m); // 10%
        result.SaldoDisponible.Should().Be(10_000_000m);
        result.Vigente.Should().BeTrue();
    }

    [Fact]
    public async Task EntregarAuxilio_WithCreditoBlando_SetsReintegro12Months()
    {
        // Arrange
        var asociadoId = Guid.NewGuid();
        var orgId = Guid.NewGuid();

        // Act
        var result = await _service.EntregarAuxilioAsync(
            asociadoId, orgId, null,
            TipoAuxilio.CreditoBlando, 3_000_000m,
            "Crédito blando para vivienda", true);

        // Assert
        result.Should().NotBeNull();
        result.Tipo.Should().Be(TipoAuxilio.CreditoBlando);
        result.Monto.Should().Be(3_000_000m);
        result.RequiereReintegro.Should().BeTrue();
        result.FechaLimiteReintegro.Should().NotBeNull();
        result.FechaLimiteReintegro.Should().BeCloseTo(
            DateTime.UtcNow.AddMonths(12),
            TimeSpan.FromDays(1));
    }
}
