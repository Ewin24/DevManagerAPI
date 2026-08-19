namespace Application.Services.SST;

using Application.DTOs.SST;
using Application.Interfaces;
using Domain.Enums;
using Microsoft.Extensions.Logging;

/// <summary>
/// Implementación del servicio de Salud Ocupacional y SST
/// SG-SST per Decreto 1072/2015 + Res. 0312/2019
/// Gestiona exámenes médicos, ARL, accidentes (FURAT) y matriz de riesgos
/// </summary>
public class SstService : ISstService
{
    private readonly ILogger<SstService> _logger;
    private readonly List<ExamenMedicoDto> _examenesStore = new();
    private readonly List<AccidenteDto> _accidentesStore = new();
    private readonly List<RiesgoDto> _riesgosStore = new();
    private static readonly Random _random = new();
    private const int DiasAlertaArl = 30;

    public SstService(ILogger<SstService> logger)
    {
        _logger = logger;
    }

    // ===== Exámenes Médicos =====

    /// <inheritdoc/>
    public Task<ExamenMedicoDto> ProgramarExamenAsync(CreateExamenMedicoDto dto)
    {
        _logger.LogInformation(
            "Programando examen {Tipo} para asociado {AsociadoId} en {Fecha}",
            dto.TipoExamen, dto.AsociadoId, dto.FechaProgramado);

        var examen = new ExamenMedicoDto
        {
            Id = Guid.NewGuid(),
            AsociadoId = dto.AsociadoId,
            OrganizationId = dto.OrganizationId,
            TipoExamen = dto.TipoExamen,
            TipoExamenNombre = dto.TipoExamen.ToString(),
            FechaProgramado = dto.FechaProgramado,
            CreatedAt = DateTime.UtcNow
        };

        _examenesStore.Add(examen);
        return Task.FromResult(examen);
    }

    /// <inheritdoc/>
    public Task<ExamenMedicoDto> RegistrarExamenAsync(
        Guid examenId, string resultado, string? archivoUrl, string? observaciones)
    {
        _logger.LogInformation(
            "Registrando resultado de examen {ExamenId}: {Resultado}",
            examenId, resultado);

        var existing = _examenesStore.FirstOrDefault(e => e.Id == examenId);
        if (existing == null)
            throw new KeyNotFoundException($"Examen {examenId} no encontrado");

        var updated = existing with
        {
            Resultado = resultado,
            ArchivoUrl = archivoUrl,
            Observaciones = observaciones,
            FechaRealizado = DateTime.UtcNow
        };

        var index = _examenesStore.IndexOf(existing);
        _examenesStore[index] = updated;

        return Task.FromResult(updated);
    }

    /// <inheritdoc/>
    public Task<List<ExamenMedicoDto>> GetExamenesByAsociadoAsync(Guid asociadoId)
    {
        var result = _examenesStore
            .Where(e => e.AsociadoId == asociadoId)
            .OrderByDescending(e => e.FechaProgramado)
            .ToList();

        return Task.FromResult(result);
    }

    /// <inheritdoc/>
    public Task<List<ExamenMedicoDto>> GetExamenesPendientesAsync(Guid organizationId)
    {
        var result = _examenesStore
            .Where(e => e.OrganizationId == organizationId && !e.Realizado)
            .OrderBy(e => e.FechaProgramado)
            .ToList();

        return Task.FromResult(result);
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
    public Task<AccidenteDto> ReportarAccidenteAsync(CreateAccidenteDto dto)
    {
        _logger.LogInformation(
            "Reportando accidente {Tipo} para asociado {AsociadoId} - {Gravedad}",
            dto.Tipo, dto.AsociadoId, dto.Gravedad);

        var accidente = new AccidenteDto
        {
            Id = Guid.NewGuid(),
            AsociadoId = dto.AsociadoId,
            OrganizationId = dto.OrganizationId,
            Fecha = dto.Fecha,
            Tipo = dto.Tipo,
            Gravedad = dto.Gravedad,
            GravedadNombre = dto.Gravedad.ToString(),
            ARL = dto.ARL,
            Descripcion = dto.Descripcion,
            FURAT = dto.FURAT,
            InvestigacionCompletada = false,
            CreatedAt = DateTime.UtcNow
        };

        _accidentesStore.Add(accidente);
        return Task.FromResult(accidente);
    }

    /// <inheritdoc/>
    public Task<List<AccidenteDto>> GetAccidentesPendientesInvestigacionAsync(Guid organizationId)
    {
        var result = _accidentesStore
            .Where(a => a.OrganizationId == organizationId && !a.InvestigacionCompletada)
            .OrderByDescending(a => a.Fecha)
            .ToList();

        return Task.FromResult(result);
    }

    /// <inheritdoc/>
    public Task<AccidenteDto> RegistrarInvestigacionAsync(
        Guid accidenteId, DateTime fechaInvestigacion,
        string conclusiones, string causas, string medidasCorrectivas)
    {
        _logger.LogInformation(
            "Registrando investigación para accidente {AccidenteId}", accidenteId);

        var existing = _accidentesStore.FirstOrDefault(a => a.Id == accidenteId);
        if (existing == null)
            throw new KeyNotFoundException($"Accidente {accidenteId} no encontrado");

        var updated = existing with
        {
            FechaInvestigacion = fechaInvestigacion,
            InvestigacionCompletada = true,
            Conclusiones = conclusiones,
            Causas = causas,
            MedidasCorrectivas = medidasCorrectivas
        };

        var index = _accidentesStore.IndexOf(existing);
        _accidentesStore[index] = updated;

        return Task.FromResult(updated);
    }

    /// <inheritdoc/>
    public Task<List<AccidenteDto>> GetAccidentesByOrganizacionAsync(Guid organizationId)
    {
        var result = _accidentesStore
            .Where(a => a.OrganizationId == organizationId)
            .OrderByDescending(a => a.Fecha)
            .ToList();

        return Task.FromResult(result);
    }

    // ===== Riesgos =====

    /// <inheritdoc/>
    public Task<List<RiesgoDto>> GetRiesgosAsync(Guid organizationId)
    {
        var result = _riesgosStore
            .Where(r => r.OrganizationId == organizationId && r.Activo)
            .ToList();

        return Task.FromResult(result);
    }

    /// <inheritdoc/>
    public Task<RiesgoDto> CrearRiesgoAsync(CreateRiesgoDto dto)
    {
        _logger.LogInformation(
            "Creando riesgo '{Factor}' nivel {Nivel} para organización {OrgId}",
            dto.Factor, dto.NivelRiesgo, dto.OrganizationId);

        var nivelNombre = dto.NivelRiesgo switch
        {
            1 => "Bajo",
            2 => "Medio",
            3 => "Alto",
            4 => "Muy Alto",
            5 => "Crítico",
            _ => "Desconocido"
        };

        var riesgo = new RiesgoDto
        {
            Id = Guid.NewGuid(),
            OrganizationId = dto.OrganizationId,
            NivelRiesgo = dto.NivelRiesgo,
            NivelRiesgoNombre = nivelNombre,
            Factor = dto.Factor,
            Descripcion = dto.Descripcion,
            Activo = true,
            Controles = dto.Controles,
            CreatedAt = DateTime.UtcNow
        };

        _riesgosStore.Add(riesgo);
        return Task.FromResult(riesgo);
    }
}
