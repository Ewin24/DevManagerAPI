namespace Application.Tests.Services;

using Application.DTOs.HabeasData;
using Application.Interfaces;
using Application.Services.HabeasData;
using Domain.Enums;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

/// <summary>
/// Tests unitarios para HabeasDataService (Fase 3 — Ley 1581/2012)
/// Autorizaciones y solicitudes ARCO
/// </summary>
public class HabeasDataServiceTests
{
    private readonly Mock<ILogger<HabeasDataService>> _loggerMock;
    private readonly IHabeasDataService _service;

    public HabeasDataServiceTests()
    {
        _loggerMock = new Mock<ILogger<HabeasDataService>>();
        var stores = new InMemoryStores();
        _service = new HabeasDataService(
            new InMemoryAutorizacionRepository(stores),
            new InMemorySolicitudARCORepository(stores),
            _loggerMock.Object);
    }

    [Fact]
    public async Task RegistrarAutorizacion_CreatesVigenteAutorizacion()
    {
        // Arrange
        var dto = new CreateAutorizacionDto
        {
            AsociadoId = Guid.NewGuid(),
            OrganizationId = Guid.NewGuid(),
            Finalidad = "Gestión de nómina, beneficios y cumplimiento normativo",
            MedioAutorizacion = "Digital"
        };

        // Act
        var result = await _service.RegistrarAutorizacionAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Vigente.Should().BeTrue();
        result.Revocada.Should().BeFalse();
        result.Finalidad.Should().Be(dto.Finalidad);
        result.MedioAutorizacion.Should().Be("Digital");
    }

    [Fact]
    public async Task RevocarAutorizacion_MarksAsRevocada()
    {
        // Arrange
        var autorizacion = await _service.RegistrarAutorizacionAsync(new CreateAutorizacionDto
        {
            AsociadoId = Guid.NewGuid(),
            OrganizationId = Guid.NewGuid(),
            Finalidad = "Finalidad de prueba",
            MedioAutorizacion = "Digital"
        });

        // Act
        var result = await _service.RevocarAutorizacionAsync(autorizacion.Id);

        // Assert
        result.Revocada.Should().BeTrue();
        result.Vigente.Should().BeFalse();
        result.FechaRevocacion.Should().NotBeNull();
    }

    [Fact]
    public async Task TieneAutorizacionVigente_AfterRegistro_ReturnsTrue()
    {
        // Arrange
        var asociadoId = Guid.NewGuid();
        await _service.RegistrarAutorizacionAsync(new CreateAutorizacionDto
        {
            AsociadoId = asociadoId,
            OrganizationId = Guid.NewGuid(),
            Finalidad = "Gestión de datos personales",
            MedioAutorizacion = "Físico"
        });

        // Act
        var tiene = await _service.TieneAutorizacionVigenteAsync(asociadoId);

        // Assert
        tiene.Should().BeTrue();
    }

    [Fact]
    public async Task CrearSolicitudARCO_Acceso_ReturnsPendiente()
    {
        // Arrange
        var dto = new CreateSolicitudARCODto
        {
            AsociadoId = Guid.NewGuid(),
            OrganizationId = Guid.NewGuid(),
            Tipo = TipoSolicitudARCO.Acceso,
            Descripcion = "Solicito acceso a todos mis datos personales registrados"
        };

        // Act
        var result = await _service.CrearSolicitudARCOAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Tipo.Should().Be(TipoSolicitudARCO.Acceso);
        result.TipoNombre.Should().Be("Acceso");
        result.Estado.Should().Be(EstadoSolicitudARCO.Pendiente);
        result.FechaRespuesta.Should().BeNull();
    }

    [Fact]
    public async Task AtenderSolicitudARCO_Cancelacion_RespondeCorrectamente()
    {
        // Arrange
        var solicitud = await _service.CrearSolicitudARCOAsync(new CreateSolicitudARCODto
        {
            AsociadoId = Guid.NewGuid(),
            OrganizationId = Guid.NewGuid(),
            Tipo = TipoSolicitudARCO.Cancelacion,
            Descripcion = "Solicito la cancelación de mis datos personales por término de vinculación"
        });

        // Act
        var result = await _service.AtenderSolicitudARCOAsync(
            solicitud.Id,
            "Se procede a la cancelación de datos personales conforme a la solicitud. " +
            "La información será bloqueada por 2 años según artículo 10 del Decreto 1377/2013.");

        // Assert
        result.Estado.Should().Be(EstadoSolicitudARCO.Atendida);
        result.Respuesta.Should().Contain("cancelación de datos personales");
        result.FechaRespuesta.Should().NotBeNull();
    }

    [Fact]
    public async Task GetSolicitudesPendientes_AfterCreacion_ReturnsSolicitud()
    {
        // Arrange
        var orgId = Guid.NewGuid();
        await _service.CrearSolicitudARCOAsync(new CreateSolicitudARCODto
        {
            AsociadoId = Guid.NewGuid(),
            OrganizationId = orgId,
            Tipo = TipoSolicitudARCO.Oposicion,
            Descripcion = "Me opongo al tratamiento de mis datos para fines comerciales"
        });

        // Act
        var pendientes = await _service.GetSolicitudesARCOPendientesAsync(orgId);

        // Assert
        pendientes.Should().HaveCount(1);
        pendientes[0].Tipo.Should().Be(TipoSolicitudARCO.Oposicion);
        pendientes[0].Estado.Should().Be(EstadoSolicitudARCO.Pendiente);
    }
}
