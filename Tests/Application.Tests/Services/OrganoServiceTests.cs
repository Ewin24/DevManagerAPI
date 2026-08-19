namespace Application.Tests.Services;

using Application.DTOs.Organos;
using Application.Interfaces;
using Application.Services.Organos;
using Domain.Enums;
using FluentAssertions;
using Xunit;

/// <summary>
/// Tests unitarios para OrganoService (Fase 4 — Órganos, Asambleas, Voto)
/// Ley 79 art.26-45
/// </summary>
public class OrganoServiceTests
{
    private readonly IOrganoService _service;

    public OrganoServiceTests()
    {
        _service = new OrganoService();
    }

    // ========= Órganos (4 scenarios) =========

    [Fact]
    public async Task CreateOrgano_WithConsejoAdministracion_ReturnsOrganoWithCorrectTipo()
    {
        // Arrange
        var dto = new CreateOrganoDto
        {
            Tipo = TipoOrgano.ConsejoAdministracion,
            Nombre = "Consejo de Administración 2026-2028",
            OrganizationId = Guid.NewGuid(),
            FechaConstitucion = new DateTime(2026, 1, 15),
            Descripcion = "Consejo de Administración periodo 2026-2028"
        };

        // Act
        var result = await _service.CreateOrganoAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Tipo.Should().Be(TipoOrgano.ConsejoAdministracion);
        result.TipoNombre.Should().Be("ConsejoAdministracion");
        result.Nombre.Should().Be("Consejo de Administración 2026-2028");
        result.Activo.Should().BeTrue();
    }

    [Fact]
    public async Task AsignarMiembro_WithPresidenteCargo_ReturnsMiembroActivo()
    {
        // Arrange
        var organoDto = new CreateOrganoDto
        {
            Tipo = TipoOrgano.JuntaVigilancia,
            Nombre = "Junta de Vigilancia 2026",
            OrganizationId = Guid.NewGuid(),
            FechaConstitucion = DateTime.UtcNow
        };
        var organo = await _service.CreateOrganoAsync(organoDto);

        // Act
        var result = await _service.AsignarMiembroAsync(new AsignarMiembroDto
        {
            OrganoId = organo.Id,
            AsociadoId = Guid.NewGuid(),
            Cargo = "Presidente",
            FechaInicio = DateTime.UtcNow
        });

        // Assert
        result.Should().NotBeNull();
        result.OrganoId.Should().Be(organo.Id);
        result.Cargo.Should().Be("Presidente");
        result.Activo.Should().BeTrue();
        result.FechaFin.Should().BeNull();
    }

    [Fact]
    public async Task GetOrganosByType_WithTipoComite_ReturnsOnlyComites()
    {
        // Arrange
        var orgId = Guid.NewGuid();

        await _service.CreateOrganoAsync(new CreateOrganoDto
        {
            Tipo = TipoOrgano.Comite, Nombre = "Comité de Educación",
            OrganizationId = orgId, FechaConstitucion = DateTime.UtcNow
        });
        await _service.CreateOrganoAsync(new CreateOrganoDto
        {
            Tipo = TipoOrgano.Comite, Nombre = "Comité de Convivencia",
            OrganizationId = orgId, FechaConstitucion = DateTime.UtcNow
        });
        await _service.CreateOrganoAsync(new CreateOrganoDto
        {
            Tipo = TipoOrgano.RevisorFiscal, Nombre = "Revisoría Fiscal",
            OrganizationId = orgId, FechaConstitucion = DateTime.UtcNow
        });

        // Act
        var comites = await _service.GetOrganosByTypeAsync(orgId, TipoOrgano.Comite);
        var revisores = await _service.GetOrganosByTypeAsync(orgId, TipoOrgano.RevisorFiscal);

        // Assert
        comites.Should().HaveCount(2);
        comites.Should().AllSatisfy(c => c.Tipo.Should().Be(TipoOrgano.Comite));
        revisores.Should().HaveCount(1);
    }

    [Fact]
    public async Task RegistrarActa_WithDecisiones_StoresAndRetrievesCorrectly()
    {
        // Arrange
        var organo = await _service.CreateOrganoAsync(new CreateOrganoDto
        {
            Tipo = TipoOrgano.ConsejoAdministracion,
            Nombre = "Consejo de Administración",
            OrganizationId = Guid.NewGuid(),
            FechaConstitucion = DateTime.UtcNow
        });

        // Act
        var acta = await _service.CreateActaAsync(new CreateActaDto
        {
            OrganoId = organo.Id,
            Fecha = DateTime.UtcNow,
            TipoSesion = "Ordinaria",
            Quorum = 5,
            Decisiones = "1. Aprobar balance anual. 2. Designar comité de crédito.",
            ActaUrl = "https://files.example.com/acta-001.pdf"
        });

        // Assert
        acta.Should().NotBeNull();
        acta.OrganoId.Should().Be(organo.Id);
        acta.TipoSesion.Should().Be("Ordinaria");
        acta.Quorum.Should().Be(5);
        acta.Decisiones.Should().Contain("balance anual");

        // Verify retrieval
        var retrieved = await _service.GetActaByIdAsync(acta.Id);
        retrieved.Should().NotBeNull();
        retrieved!.ActaUrl.Should().Be("https://files.example.com/acta-001.pdf");
    }

    // ========= Asambleas (2 scenarios) =========

    [Fact]
    public async Task ConvocarAsamblea_WithExtraordinaria_CreatesOpenAsamblea()
    {
        // Arrange
        var orgId = Guid.NewGuid();

        // Act
        var result = await _service.ConvocarAsambleaAsync(new ConvocarAsambleaDto
        {
            OrganizationId = orgId,
            Fecha = new DateTime(2026, 8, 15),
            Tipo = TipoAsamblea.Extraordinaria,
            Convocatoria = "Asamblea Extraordinaria para aprobar distribución de excedentes",
            QuorumMinimo = 30
        });

        // Assert
        result.Should().NotBeNull();
        result.Tipo.Should().Be(TipoAsamblea.Extraordinaria);
        result.TipoNombre.Should().Be("Extraordinaria");
        result.Cerrada.Should().BeFalse();
        result.QuorumMinimo.Should().Be(30);
        result.Asistentes.Should().BeNull();
    }

    [Fact]
    public async Task CerrarAsamblea_WithResultados_MarksAsClosed()
    {
        // Arrange
        var asamblea = await _service.ConvocarAsambleaAsync(new ConvocarAsambleaDto
        {
            OrganizationId = Guid.NewGuid(),
            Fecha = DateTime.UtcNow,
            Tipo = TipoAsamblea.Ordinaria,
            Convocatoria = "Asamblea Ordinaria Anual",
            QuorumMinimo = 20
        });

        await _service.RegistrarAsistenciaAsync(asamblea.Id, new RegistrarAsistenciaDto
        {
            Asistentes = 35
        });

        // Act
        var result = await _service.CerrarAsambleaAsync(asamblea.Id, new CerrarAsambleaDto
        {
            Resultados = "Aprobado balance social 2025 por unanimidad"
        });

        // Assert
        result.Cerrada.Should().BeTrue();
        result.FechaCierre.Should().NotBeNull();
        result.Asistentes.Should().Be(35);
        result.Resultados.Should().Contain("Aprobado balance social");
    }

    // ========= Voto (3 scenarios) =========

    [Fact]
    public async Task EmitirVoto_WithAprobado_RegistersCorrectly()
    {
        // Arrange
        var asamblea = await _service.ConvocarAsambleaAsync(new ConvocarAsambleaDto
        {
            OrganizationId = Guid.NewGuid(),
            Fecha = DateTime.UtcNow,
            Tipo = TipoAsamblea.Ordinaria,
            Convocatoria = "Votación de reforma estatutaria",
            QuorumMinimo = 25
        });

        var asociadoId = Guid.NewGuid();

        // Act
        var result = await _service.EmitirVotoAsync(new EmitirVotoDto
        {
            AsambleaId = asamblea.Id,
            AsociadoId = asociadoId,
            VotoEmitido = TipoVoto.Aprobado
        });

        // Assert
        result.Should().NotBeNull();
        result.AsambleaId.Should().Be(asamblea.Id);
        result.AsociadoId.Should().Be(asociadoId);
        result.VotoEmitido.Should().Be(TipoVoto.Aprobado);
        result.VotoNombre.Should().Be("Aprobado");
    }

    [Fact]
    public async Task EmitirVoto_DobleVoto_ThrowsInvalidOperation()
    {
        // Arrange
        var asamblea = await _service.ConvocarAsambleaAsync(new ConvocarAsambleaDto
        {
            OrganizationId = Guid.NewGuid(),
            Fecha = DateTime.UtcNow,
            Tipo = TipoAsamblea.Ordinaria,
            Convocatoria = "Votación única",
            QuorumMinimo = 10
        });

        var asociadoId = Guid.NewGuid();
        await _service.EmitirVotoAsync(new EmitirVotoDto
        {
            AsambleaId = asamblea.Id,
            AsociadoId = asociadoId,
            VotoEmitido = TipoVoto.Aprobado
        });

        // Act
        Func<Task> act = () => _service.EmitirVotoAsync(new EmitirVotoDto
        {
            AsambleaId = asamblea.Id,
            AsociadoId = asociadoId,
            VotoEmitido = TipoVoto.Rechazado
        });

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*ya ha votado*");
    }

    [Fact]
    public async Task GetResultados_WithMultipleVotos_ReturnsCorrectCounts()
    {
        // Arrange
        var asamblea = await _service.ConvocarAsambleaAsync(new ConvocarAsambleaDto
        {
            OrganizationId = Guid.NewGuid(),
            Fecha = DateTime.UtcNow,
            Tipo = TipoAsamblea.Ordinaria,
            Convocatoria = "Elección del Consejo de Administración",
            QuorumMinimo = 50
        });

        var asociado1 = Guid.NewGuid();
        var asociado2 = Guid.NewGuid();
        var asociado3 = Guid.NewGuid();
        var asociado4 = Guid.NewGuid();
        var asociado5 = Guid.NewGuid();

        await _service.EmitirVotoAsync(new EmitirVotoDto { AsambleaId = asamblea.Id, AsociadoId = asociado1, VotoEmitido = TipoVoto.Aprobado });
        await _service.EmitirVotoAsync(new EmitirVotoDto { AsambleaId = asamblea.Id, AsociadoId = asociado2, VotoEmitido = TipoVoto.Aprobado });
        await _service.EmitirVotoAsync(new EmitirVotoDto { AsambleaId = asamblea.Id, AsociadoId = asociado3, VotoEmitido = TipoVoto.Aprobado });
        await _service.EmitirVotoAsync(new EmitirVotoDto { AsambleaId = asamblea.Id, AsociadoId = asociado4, VotoEmitido = TipoVoto.Rechazado });
        await _service.EmitirVotoAsync(new EmitirVotoDto { AsambleaId = asamblea.Id, AsociadoId = asociado5, VotoEmitido = TipoVoto.Abstencion });

        // Act
        var resultados = await _service.GetResultadosAsync(asamblea.Id);

        // Assert
        resultados.TotalVotos.Should().Be(5);
        resultados.Aprobados.Should().Be(3);
        resultados.Rechazados.Should().Be(1);
        resultados.Abstenciones.Should().Be(1);
        resultados.Blancos.Should().Be(0);
    }
}
