namespace Application.Tests.Services;

using Application.DTOs.Agent;
using Application.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

/// <summary>
/// Tests unitarios para CooperativaAIService (Fase 5 — Asistente Cooperativo IA)
/// Basado en Ley 79/1988, Ley 454/1998, Circular Básica Jurídica 2020
/// </summary>
public class CooperativaAIServiceTests
{
    private readonly CooperativaAIService _service;
    private readonly Guid _orgId = Guid.NewGuid();

    public CooperativaAIServiceTests()
    {
        var loggerMock = new Mock<ILogger<CooperativaAIService>>();
        _service = new CooperativaAIService(loggerMock.Object);
    }

    [Fact]
    public async Task ConsultarNormatividad_WithExcedentesQuery_ReturnsLey79Art54()
    {
        // Arrange
        var request = new CooperativaQueryRequest
        {
            Consulta = "¿Cómo se distribuyen los excedentes en una cooperativa?",
            RequerirAprobacion = false
        };

        // Act
        var result = await _service.ConsultarNormatividadAsync(_orgId, request);

        // Assert
        result.Should().NotBeNull();
        result.Citations.Should().NotBeEmpty();
        result.Citations.Should().Contain(c =>
            c.Norma.Contains("Ley 79") && c.Articulo.Contains("Art. 54"));
        result.Respuesta.Should().NotBeNullOrEmpty();
        result.AccionesSugeridas.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GenerarBalanceSocial_WithValidYear_ReturnsEightDimensions()
    {
        // Arrange
        var request = new GenerarBalanceSocialRequest
        {
            OrganizationId = _orgId,
            Anio = 2026,
            IncluirRecomendaciones = true
        };

        // Act
        var result = await _service.GenerarBalanceSocialAsync(_orgId, request);

        // Assert
        result.Should().NotBeNull();
        result.Dimensiones.Should().HaveCount(8);
        result.Anio.Should().Be(2026);
        result.ResumenEjecutivo.Should().NotBeNullOrEmpty();
        result.Fortalezas.Should().NotBeEmpty();
        result.OportunidadesMejora.Should().NotBeEmpty();

        // Verificar dimensiones esperadas
        result.Dimensiones.Select(d => d.Nombre).Should().Contain(new[]
        {
            "Gobernanza Democrática",
            "Educación e Información",
            "Ética y Transparencia",
            "Desarrollo Económico"
        });

        // Cada dimensión debe tener indicadores
        result.Dimensiones.Should().AllSatisfy(d =>
        {
            d.Indicadores.Should().NotBeEmpty();
            d.Estado.Should().NotBeNullOrEmpty();
        });
    }

    [Fact]
    public async Task VerificarCumplimiento_WithAllAreasGreen_ReturnsPassingStatus()
    {
        // Arrange
        var request = new VerificarCumplimientoRequest
        {
            OrganizationId = _orgId,
            VerificarEducacion = true,
            VerificarSST = true,
            VerificarHabeasData = true,
            VerificarAportes = true
        };

        // Act
        var result = await _service.VerificarCumplimientoAsync(_orgId, request);

        // Assert
        result.Should().NotBeNull();
        result.Areas.Should().HaveCount(4);
        result.AreasCumplen.Should().BeGreaterThan(0);
        result.CoberturaGeneral.Should().BeGreaterThan(0);

        // SST, HabeasData y Aportes deberían cumplir
        result.Areas.First(a => a.Nombre.Contains("Seguridad")).Cumple.Should().BeTrue();
        result.Areas.First(a => a.Nombre.Contains("Datos")).Cumple.Should().BeTrue();
        result.Areas.First(a => a.Nombre.Contains("Aportes")).Cumple.Should().BeTrue();
    }

    [Fact]
    public async Task VerificarCumplimiento_WithEducacionDeficiency_ReturnsAlert()
    {
        // Arrange
        var request = new VerificarCumplimientoRequest
        {
            OrganizationId = _orgId,
            VerificarEducacion = true,
            VerificarSST = false,
            VerificarHabeasData = false,
            VerificarAportes = false
        };

        // Act
        var result = await _service.VerificarCumplimientoAsync(_orgId, request);

        // Assert
        result.Should().NotBeNull();
        result.Areas.Should().HaveCount(1);
        var educacion = result.Areas[0];

        // Educación no cumple por cobertura insuficiente (simulado)
        educacion.Cumple.Should().BeFalse();
        educacion.NormaAplicable.Should().Contain("Ley 79");
        educacion.Hallazgos.Should().Contain(h => h.Contains("insuficiente") || h.Contains("subutilizado"));

        // Debería haber una alerta general
        result.Alertas.Should().Contain(a => a.Contains("EDUCACIÓN"));
        result.CoberturaGeneral.Should().Be(0); // Single area, no cumple
    }

    [Fact]
    public async Task ResponderDudaAsociado_WithAfilacionQuery_ReturnsDerechosResponse()
    {
        // Arrange
        var request = new ResponderDudaRequest
        {
            OrganizationId = _orgId,
            Pregunta = "¿Cómo me afilio a la cooperativa?",
            TipoAsociado = "nuevo"
        };

        // Act
        var result = await _service.ResponderDudaAsociadoAsync(_orgId, request);

        // Assert
        result.Should().NotBeNull();
        result.Respuesta.Should().NotBeNullOrEmpty();
        result.Respuesta.Should().Contain("Ley 79");
        result.Citations.Should().NotBeEmpty();
        result.Citations[0].Norma.Should().Be("Ley 79");
        result.AccionesSugeridas.Should().NotBeEmpty();
    }

    [Fact]
    public async Task ResponderDudaAsociado_WithExcedentesQuery_ReturnsBeneficiosResponse()
    {
        // Arrange
        var request = new ResponderDudaRequest
        {
            OrganizationId = _orgId,
            Pregunta = "¿Cómo se distribuyen los excedentes?",
            TipoAsociado = "asociado"
        };

        // Act
        var result = await _service.ResponderDudaAsociadoAsync(_orgId, request);

        // Assert
        result.Should().NotBeNull();
        result.Respuesta.Should().NotBeNullOrEmpty();
        result.Respuesta.Should().Contain("20%");
        result.Citations.Should().NotBeEmpty();
        result.Citations[0].Articulo.Should().Be("Art. 54");
    }
}
