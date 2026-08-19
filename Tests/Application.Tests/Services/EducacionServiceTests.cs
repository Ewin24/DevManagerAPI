namespace Application.Tests.Services;

using Application.DTOs.GestionHumana;
using Application.Interfaces;
using Application.Services.GestionHumana;
using Domain.Enums;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

/// <summary>
/// Tests unitarios para EducacionService (Fase 2 — Educación cooperativa art.88-91)
/// </summary>
public class EducacionServiceTests
{
    private readonly Mock<ILogger<EducacionService>> _loggerMock;
    private readonly IEducacionService _service;

    public EducacionServiceTests()
    {
        _loggerMock = new Mock<ILogger<EducacionService>>();
        _service = new EducacionService(_loggerMock.Object);
    }

    [Fact]
    public async Task CreatePrograma_WithValidData_ReturnsPrograma()
    {
        // Arrange
        var dto = new CreateProgramaEducacionDto
        {
            OrganizationId = Guid.NewGuid(),
            Nombre = "Inducción Cooperativa Básica",
            Descripcion = "Fundamentos del cooperativismo",
            Tipo = TipoEducacion.Basica,
            Horas = 20,
            EsObligatorio = true,
            FechaInicio = new DateTime(2026, 1, 15)
        };

        // Act
        var result = await _service.CreateProgramaAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Nombre.Should().Be("Inducción Cooperativa Básica");
        result.Tipo.Should().Be(TipoEducacion.Basica);
        result.Horas.Should().Be(20);
        result.EsObligatorio.Should().BeTrue();
        result.Activo.Should().BeTrue();
    }

    [Fact]
    public async Task Inscribir_WithValidData_ReturnsInscripcion()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var asociadoId = Guid.NewGuid();

        var programaDto = new CreateProgramaEducacionDto
        {
            OrganizationId = orgId,
            Nombre = "Gobierno Cooperativo",
            Tipo = TipoEducacion.Avanzada,
            Horas = 40,
            EsObligatorio = false,
            FechaInicio = DateTime.UtcNow
        };

        var programa = await _service.CreateProgramaAsync(programaDto);

        var inscripcionDto = new CreateAsociadoEducacionDto
        {
            AsociadoId = asociadoId,
            ProgramaEducacionId = programa.Id,
            OrganizationId = orgId
        };

        // Act
        var result = await _service.InscribirAsync(inscripcionDto);

        // Assert
        result.Should().NotBeNull();
        result.AsociadoId.Should().Be(asociadoId);
        result.ProgramaEducacionId.Should().Be(programa.Id);
        result.Completado.Should().BeFalse();
        result.HorasCursadas.Should().Be(0);
    }

    [Fact]
    public async Task RegistrarProgreso_WithFullCompletion_MarksAsCompleted()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        var asociadoId = Guid.NewGuid();

        var programaDto = new CreateProgramaEducacionDto
        {
            OrganizationId = orgId,
            Nombre = "Curso Básico",
            Tipo = TipoEducacion.Basica,
            Horas = 20,
            EsObligatorio = true,
            FechaInicio = DateTime.UtcNow
        };

        var programa = await _service.CreateProgramaAsync(programaDto);

        var inscripcion = await _service.InscribirAsync(new CreateAsociadoEducacionDto
        {
            AsociadoId = asociadoId,
            ProgramaEducacionId = programa.Id,
            OrganizationId = orgId
        });

        // Act
        var result = await _service.RegistrarProgresoAsync(
            inscripcion.Id, 20, "Aprobado");

        // Assert
        result.Completado.Should().BeTrue();
        result.HorasCursadas.Should().Be(20);
        result.Progreso.Should().Be(100);
        result.Resultado.Should().Be("Aprobado");
        result.FechaCompletado.Should().NotBeNull();
    }

    [Fact]
    public async Task CumpleMinimoHoras_WithNoEducation_ReturnsFalse()
    {
        // Arrange
        var asociadoId = Guid.NewGuid();

        // Act
        var result = await _service.CumpleMinimoHorasAsync(asociadoId, 2026);

        // Assert
        result.Should().BeFalse();
    }
}
