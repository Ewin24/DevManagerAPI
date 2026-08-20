namespace Application.Tests.Services;

using Application.DTOs.Reportes;
using Application.Interfaces;
using Application.Services.Reportes;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

/// <summary>
/// Tests unitarios para ReportGeneratorService (Fase 3 — Supersolidaria Reports)
/// Compila Balance Social, asociados y cumplimiento SST
/// </summary>
public class ReportGeneratorServiceTests
{
    private readonly Mock<ILogger<ReportGeneratorService>> _loggerMock;
    private readonly IReportGeneratorService _service;

    public ReportGeneratorServiceTests()
    {
        _loggerMock = new Mock<ILogger<ReportGeneratorService>>();
        _service = new ReportGeneratorService(
            new InMemoryReporteSupersolidariaRepository(new InMemoryStores()),
            _loggerMock.Object);
    }

    [Fact]
    public async Task GenerarReporte_WithValidData_ReturnsCompleteReport()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var periodo = new DateTime(2026, 6, 30);

        // Act
        var result = await _service.GenerarReporteAsync(orgId, periodo, "Trimestral");

        // Assert
        result.Should().NotBeNull();
        result.OrganizationId.Should().Be(orgId);
        result.Periodo.Should().Be(periodo);
        result.TipoReporte.Should().Be("Trimestral");
        result.Enviado.Should().BeFalse();

        // Verificar que los JSONs contienen datos esperados
        result.BalanceSocialJson.Should().NotBeNullOrEmpty();
        result.BalanceSocialJson.Should().Contain("GobernanzaDemocratica");
        result.BalanceSocialJson.Should().Contain("cobertura");

        result.AsociadosJson.Should().NotBeNullOrEmpty();
        result.AsociadosJson.Should().Contain("totalAsociados");

        result.CumplimientoJson.Should().NotBeNullOrEmpty();
        result.CumplimientoJson.Should().Contain("arlVigente");
        result.CumplimientoJson.Should().Contain("excedentesDistribuidos");
    }

    [Fact]
    public async Task GenerarYEnviarReporte_MarksAsEnviado()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var reporte = await _service.GenerarReporteAsync(orgId, new DateTime(2026, 3, 31), "Trimestral");

        // Act
        var enviado = await _service.MarcarEnviadoAsync(reporte.Id);

        // Assert
        enviado.Enviado.Should().BeTrue();
        enviado.FechaEnvio.Should().NotBeNull();

        // Verificar consulta por período
        var consultado = await _service.GetReporteByPeriodoAsync(orgId, new DateTime(2026, 3, 31));
        consultado.Should().NotBeNull();
        consultado!.Enviado.Should().BeTrue();
    }

    [Fact]
    public async Task GetReportesByOrganizacion_ReturnsAllReports()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        await _service.GenerarReporteAsync(orgId, new DateTime(2026, 3, 31), "Trimestral");
        await _service.GenerarReporteAsync(orgId, new DateTime(2026, 6, 30), "Trimestral");

        // Act
        var reportes = await _service.GetReportesByOrganizacionAsync(orgId);

        // Assert
        reportes.Should().HaveCount(2);
        reportes.Should().BeInDescendingOrder(r => r.Periodo);
    }
}
