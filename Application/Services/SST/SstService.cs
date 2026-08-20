namespace Application.Services.SST;

using Application.DTOs.SST;
using Application.Interfaces;
using Domain.Entities.SST;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

/// <summary>
/// Implementación del servicio de Salud Ocupacional y SST
/// SG-SST per Decreto 1072/2015 + Res. 0312/2019
/// Gestiona exámenes médicos, ARL, accidentes (FURAT) y matriz de riesgos
/// </summary>
public class SstService : ISstService
{
    private readonly IExamenMedicoRepository _examenesRepository;
    private readonly IAccidenteRepository _accidentesRepository;
    private readonly IRiesgoRepository _riesgosRepository;
    private readonly ILogger<SstService> _logger;

    private static readonly Random _random = new();
    private const int DiasAlertaArl = 30;

    public SstService(
        IExamenMedicoRepository examenesRepository,
        IAccidenteRepository accidentesRepository,
        IRiesgoRepository riesgosRepository,
        ILogger<SstService> logger)
    {
        _examenesRepository = examenesRepository;
        _accidentesRepository = accidentesRepository;
        _riesgosRepository = riesgosRepository;
        _logger = logger;
    }

    // ===== Exámenes Médicos =====

    /// <inheritdoc/>
    public async Task<ExamenMedicoDto> ProgramarExamenAsync(CreateExamenMedicoDto dto)
    {
        _logger.LogInformation(
            "Programando examen {Tipo} para asociado {AsociadoId} en {Fecha}",
            dto.TipoExamen, dto.AsociadoId, dto.FechaProgramado);

        var examen = new ExamenMedico
        {
            Id = Guid.NewGuid(),
            AsociadoId = dto.AsociadoId,
            OrganizationId = dto.OrganizationId,
            TipoExamen = dto.TipoExamen,
            FechaProgramado = dto.FechaProgramado,
            CreatedAt = DateTime.UtcNow
        };

        var creado = await _examenesRepository.CreateAsync(examen);
        return MapToDto(creado);
    }

    /// <inheritdoc/>
    public async Task<ExamenMedicoDto> RegistrarExamenAsync(
        Guid examenId, string resultado, string? archivoUrl, string? observaciones)
    {
        _logger.LogInformation(
            "Registrando resultado de examen {ExamenId}: {Resultado}",
            examenId, resultado);

        var existing = await _examenesRepository.GetByIdAsync(examenId);
        if (existing == null)
            throw new KeyNotFoundException($"Examen {examenId} no encontrado");

        existing.Resultado = resultado;
        existing.ArchivoUrl = archivoUrl;
        existing.Observaciones = observaciones;
        existing.FechaRealizado = DateTime.UtcNow;

        var updated = await _examenesRepository.UpdateAsync(existing);
        return MapToDto(updated);
    }

    /// <inheritdoc/>
    public async Task<List<ExamenMedicoDto>> GetExamenesByAsociadoAsync(Guid asociadoId)
    {
        var examenes = await _examenesRepository.GetByAsociadoAsync(asociadoId);
        return examenes.Select(MapToDto).ToList();
    }

    /// <inheritdoc/>
    public async Task<List<ExamenMedicoDto>> GetExamenesPendientesAsync(Guid organizationId)
    {
        var examenes = await _examenesRepository.GetPendientesAsync(organizationId);
        return examenes.Select(MapToDto).ToList();
    }

    // ===== ARL Vigencia =====

    /// <inheritdoc/>
    public Task<(bool Vigente, int DiasRestantes, string? Alerta)> VerificarVigenciaArlAsync(Guid organizationId)
    {
        _logger.LogInformation("Verificando vigencia ARL para organización {OrgId}", organizationId);

        // Simula ARL registration: asumimos 180 días de vigencia desde created
        // En un sistema real, se consultaría el registro de ARL con vigencia
        var diasRestantes = _random.Next(0, 365);
        var vigente = diasRestantes > 0;

        string? alerta = null;
        if (!vigente)
        {
            alerta = "⚠️ La ARL ha expirado. Contrate una nueva póliza inmediatamente.";
        }
        else if (diasRestantes <= DiasAlertaArl)
        {
            alerta = $"⚠️ La ARL vence en {diasRestantes} días. Contacte a su asesor ARL para renovación.";
        }

        return Task.FromResult((vigente, diasRestantes, alerta));
    }

    // ===== Accidentes =====

    /// <inheritdoc/>
    public async Task<AccidenteDto> ReportarAccidenteAsync(CreateAccidenteDto dto)
    {
        _logger.LogInformation(
            "Reportando accidente {Tipo} para asociado {AsociadoId} - {Gravedad}",
            dto.Tipo, dto.AsociadoId, dto.Gravedad);

        var accidente = new Accidente
        {
            Id = Guid.NewGuid(),
            AsociadoId = dto.AsociadoId,
            OrganizationId = dto.OrganizationId,
            Fecha = dto.Fecha,
            Tipo = dto.Tipo,
            Gravedad = dto.Gravedad,
            ARL = dto.ARL,
            Descripcion = dto.Descripcion,
            FURAT = dto.FURAT,
            InvestigacionCompletada = false,
            CreatedAt = DateTime.UtcNow
        };

        var creado = await _accidentesRepository.CreateAsync(accidente);
        return MapToDto(creado);
    }

    /// <inheritdoc/>
    public async Task<List<AccidenteDto>> GetAccidentesPendientesInvestigacionAsync(Guid organizationId)
    {
        var accidentes = await _accidentesRepository.GetPendientesInvestigacionAsync(organizationId);
        return accidentes.Select(MapToDto).ToList();
    }

    /// <inheritdoc/>
    public async Task<AccidenteDto> RegistrarInvestigacionAsync(
        Guid accidenteId, DateTime fechaInvestigacion,
        string conclusiones, string causas, string medidasCorrectivas)
    {
        _logger.LogInformation(
            "Registrando investigación para accidente {AccidenteId}", accidenteId);

        var existing = await _accidentesRepository.GetByIdAsync(accidenteId);
        if (existing == null)
            throw new KeyNotFoundException($"Accidente {accidenteId} no encontrado");

        existing.FechaInvestigacion = fechaInvestigacion;
        existing.InvestigacionCompletada = true;
        existing.Conclusiones = conclusiones;
        existing.Causas = causas;
        existing.MedidasCorrectivas = medidasCorrectivas;

        var updated = await _accidentesRepository.UpdateAsync(existing);
        return MapToDto(updated);
    }

    /// <inheritdoc/>
    public async Task<List<AccidenteDto>> GetAccidentesByOrganizacionAsync(Guid organizationId)
    {
        var accidentes = await _accidentesRepository.GetByOrganizationAsync(organizationId);
        return accidentes.Select(MapToDto).ToList();
    }

    // ===== Riesgos =====

    /// <inheritdoc/>
    public async Task<List<RiesgoDto>> GetRiesgosAsync(Guid organizationId)
    {
        var riesgos = await _riesgosRepository.GetByOrganizationAsync(organizationId);
        return riesgos.Select(MapToDto).ToList();
    }

    /// <inheritdoc/>
    public async Task<RiesgoDto> CrearRiesgoAsync(CreateRiesgoDto dto)
    {
        _logger.LogInformation(
            "Creando riesgo '{Factor}' nivel {Nivel} para organización {OrgId}",
            dto.Factor, dto.NivelRiesgo, dto.OrganizationId);

        var nivelNombre = ObtenerNivelNombre(dto.NivelRiesgo);

        var riesgo = new Riesgo
        {
            Id = Guid.NewGuid(),
            OrganizationId = dto.OrganizationId,
            NivelRiesgo = dto.NivelRiesgo,
            Factor = dto.Factor,
            Descripcion = dto.Descripcion,
            Activo = true,
            Controles = dto.Controles,
            CreatedAt = DateTime.UtcNow
        };

        var creado = await _riesgosRepository.CreateAsync(riesgo);
        return MapToDto(creado);
    }

    // ===== Mapping =====

    private static string ObtenerNivelNombre(int nivelRiesgo) => nivelRiesgo switch
    {
        1 => "Bajo",
        2 => "Medio",
        3 => "Alto",
        4 => "Muy Alto",
        5 => "Crítico",
        _ => "Desconocido"
    };

    private static ExamenMedicoDto MapToDto(ExamenMedico e) => new()
    {
        Id = e.Id,
        AsociadoId = e.AsociadoId,
        OrganizationId = e.OrganizationId,
        TipoExamen = e.TipoExamen,
        TipoExamenNombre = e.TipoExamen.ToString(),
        FechaProgramado = e.FechaProgramado,
        FechaRealizado = e.FechaRealizado,
        Resultado = e.Resultado,
        ArchivoUrl = e.ArchivoUrl,
        Observaciones = e.Observaciones,
        CreatedAt = e.CreatedAt
    };

    private static AccidenteDto MapToDto(Accidente a) => new()
    {
        Id = a.Id,
        AsociadoId = a.AsociadoId,
        OrganizationId = a.OrganizationId,
        Fecha = a.Fecha,
        Tipo = a.Tipo,
        Gravedad = a.Gravedad,
        GravedadNombre = a.Gravedad.ToString(),
        ARL = a.ARL,
        Descripcion = a.Descripcion,
        FURAT = a.FURAT,
        FechaInvestigacion = a.FechaInvestigacion,
        InvestigacionCompletada = a.InvestigacionCompletada,
        Conclusiones = a.Conclusiones,
        Causas = a.Causas,
        MedidasCorrectivas = a.MedidasCorrectivas,
        CreatedAt = a.CreatedAt
    };

    private static RiesgoDto MapToDto(Riesgo r) => new()
    {
        Id = r.Id,
        OrganizationId = r.OrganizationId,
        NivelRiesgo = r.NivelRiesgo,
        NivelRiesgoNombre = ObtenerNivelNombre(r.NivelRiesgo),
        Factor = r.Factor,
        Descripcion = r.Descripcion,
        Activo = r.Activo,
        Controles = r.Controles,
        CreatedAt = r.CreatedAt
    };
}