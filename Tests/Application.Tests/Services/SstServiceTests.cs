namespace Application.Tests.Services;

using Application.DTOs.SST;
using Application.Interfaces;
using Application.Services.SST;
using Domain.Enums;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

/// <summary>
/// Tests unitarios para SstService (Fase 3 — Salud Ocupacional SST)
/// Res. 0312/2019 + Decreto 1072/2015 — Exámenes, ARL, accidentes, riesgos
/// </summary>
public class SstServiceTests
{
    private readonly Mock<ILogger<SstService>> _loggerMock;
    private readonly ISstService _service;

    public SstServiceTests()
    {
        _loggerMock = new Mock<ILogger<SstService>>();
        _service = new SstService(_loggerMock.Object);
    }

    [Fact]
    public async Task ProgramarExamen_WithIngresoType_ReturnsExamenProgramado()
    {
        // Arrange
        var dto = new CreateExamenMedicoDto
        {
            AsociadoId = Guid.NewGuid(),
            OrganizationId = Guid.NewGuid(),
            TipoExamen = TipoExamenMedico.Ingreso,
            FechaProgramado = DateTime.UtcNow.AddDays(15)
        };

        // Act
        var result = await _service.ProgramarExamenAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.TipoExamen.Should().Be(TipoExamenMedico.Ingreso);
        result.TipoExamenNombre.Should().Be("Ingreso");
        result.FechaProgramado.Should().BeCloseTo(dto.FechaProgramado, TimeSpan.FromSeconds(1));
        result.Realizado.Should().BeFalse();
    }

    [Fact]
    public async Task RegistrarExamen_WithValidId_UpdatesResultado()
    {
        // Arrange
        var examen = await _service.ProgramarExamenAsync(new CreateExamenMedicoDto
        {
            AsociadoId = Guid.NewGuid(),
            OrganizationId = Guid.NewGuid(),
            TipoExamen = TipoExamenMedico.Periodico,
            FechaProgramado = DateTime.UtcNow.AddDays(-5)
        });

        // Act
        var result = await _service.RegistrarExamenAsync(
            examen.Id, "Apto", "https://files.example.com/examen.pdf", "Sin hallazgos relevantes");

        // Assert
        result.Should().NotBeNull();
        result.Resultado.Should().Be("Apto");
        result.ArchivoUrl.Should().Be("https://files.example.com/examen.pdf");
        result.Observaciones.Should().Be("Sin hallazgos relevantes");
        result.Realizado.Should().BeTrue();
        result.FechaRealizado.Should().NotBeNull();
    }

    [Fact]
    public async Task VerificarVigenciaArl_ReturnsStatus()
    {
        // Arrange
        var orgId = Guid.NewGuid();

        // Act
        var (vigente, diasRestantes, alerta) = await _service.VerificarVigenciaArlAsync(orgId);

        // Assert
        diasRestantes.Should().BeGreaterThanOrEqualTo(0);
        if (diasRestantes <= 30 && diasRestantes > 0)
        {
            alerta.Should().NotBeNull();
            alerta.Should().Contain("renovación");
        }
    }

    [Fact]
    public async Task ReportarAccidente_RegistraInvestigacion_CompletaCiclo()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var accidente = await _service.ReportarAccidenteAsync(new CreateAccidenteDto
        {
            AsociadoId = Guid.NewGuid(),
            OrganizationId = orgId,
            Fecha = DateTime.UtcNow.AddDays(-3),
            Tipo = "Caída desde escalera",
            Gravedad = GravedadAccidente.Grave,
            ARL = "ARL Sura",
            Descripcion = "Caída desde escalera de 2 metros durante mantenimiento",
            FURAT = "FURAT-2026-0042"
        });

        // Act - registrar investigación dentro de 15 días hábiles
        var investigado = await _service.RegistrarInvestigacionAsync(
            accidente.Id,
            DateTime.UtcNow.AddDays(5),
            "Accidente verificado. Se implementarán medidas correctivas.",
            "Falta de mantenimiento preventivo en escalera",
            "1. Inspección de todas las escaleras. 2. Capacitación en alturas. 3. EPP obligatorio.");

        // Assert
        investigado.InvestigacionCompletada.Should().BeTrue();
        investigado.Conclusiones.Should().NotBeNullOrEmpty();
        investigado.Causas.Should().Contain("mantenimiento");
        investigado.MedidasCorrectivas.Should().Contain("Capacitación");

        // Verificar accidentes pendientes (ya no debe aparecer)
        var pendientes = await _service.GetAccidentesPendientesInvestigacionAsync(orgId);
        pendientes.Should().NotContain(a => a.Id == accidente.Id);
    }

    [Fact]
    public async Task CrearRiesgo_WithNivel5_ReturnsCritico()
    {
        // Arrange
        var dto = new CreateRiesgoDto
        {
            OrganizationId = Guid.NewGuid(),
            NivelRiesgo = 5,
            Factor = "Químico",
            Descripcion = "Exposición a solventes orgánicos sin ventilación adecuada",
            Controles = "Sistema de extracción forzada + EPP mascarilla con filtro"
        };

        // Act
        var result = await _service.CrearRiesgoAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.NivelRiesgo.Should().Be(5);
        result.NivelRiesgoNombre.Should().Be("Crítico");
        result.Factor.Should().Be("Químico");
        result.Activo.Should().BeTrue();
    }
}
