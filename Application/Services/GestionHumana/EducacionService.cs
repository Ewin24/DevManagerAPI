namespace Application.Services.GestionHumana;

using Application.DTOs.GestionHumana;
using Application.Interfaces;
using Domain.Enums;
using Microsoft.Extensions.Logging;

/// <summary>
/// Implementación del servicio de educación cooperativa
/// Gestión de programas, inscripciones y cumplimiento de horas mínimas
/// Mínimo 20 horas anuales según art.88-91 normativa solidaria
/// </summary>
public class EducacionService : IEducacionService
{
    private readonly ILogger<EducacionService> _logger;

    // Almacenes en memoria para simulación
    private readonly List<ProgramaEducacionDto> _programasStore = new();
    private readonly List<AsociadoEducacionDto> _inscripcionesStore = new();

    private const int HorasMinimasAnuales = 20;

    public EducacionService(ILogger<EducacionService> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc/>
    public Task<List<ProgramaEducacionDto>> GetProgramasAsync(Guid organizationId)
    {
        _logger.LogInformation("Obteniendo programas educativos de organización {OrgId}", organizationId);

        var result = _programasStore
            .Where(p => p.OrganizationId == organizationId)
            .ToList();

        return Task.FromResult(result);
    }

    /// <inheritdoc/>
    public Task<ProgramaEducacionDto> CreateProgramaAsync(CreateProgramaEducacionDto dto)
    {
        _logger.LogInformation(
            "Creando programa educativo '{Nombre}', tipo {Tipo}, {Horas} horas",
            dto.Nombre, dto.Tipo, dto.Horas);

        var programa = new ProgramaEducacionDto
        {
            Id = Guid.NewGuid(),
            OrganizationId = dto.OrganizationId,
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion,
            Tipo = dto.Tipo,
            Horas = dto.Horas,
            EsObligatorio = dto.EsObligatorio,
            FechaInicio = dto.FechaInicio,
            FechaFin = dto.FechaFin,
            Activo = true,
            CreatedAt = DateTime.UtcNow
        };

        _programasStore.Add(programa);
        return Task.FromResult(programa);
    }

    /// <inheritdoc/>
    public Task<AsociadoEducacionDto> InscribirAsync(CreateAsociadoEducacionDto dto)
    {
        _logger.LogInformation(
            "Inscribiendo asociado {AsociadoId} en programa {ProgramaId}",
            dto.AsociadoId, dto.ProgramaEducacionId);

        var programa = _programasStore.FirstOrDefault(p => p.Id == dto.ProgramaEducacionId);

        var inscripcion = new AsociadoEducacionDto
        {
            Id = Guid.NewGuid(),
            AsociadoId = dto.AsociadoId,
            ProgramaEducacionId = dto.ProgramaEducacionId,
            ProgramaNombre = programa?.Nombre,
            TipoEducacion = programa?.Tipo.ToString(),
            HorasPrograma = programa?.Horas ?? 0,
            HorasCursadas = 0,
            Progreso = 0,
            FechaInscripcion = DateTime.UtcNow,
            Completado = false
        };

        _inscripcionesStore.Add(inscripcion);
        return Task.FromResult(inscripcion);
    }

    /// <inheritdoc/>
    public Task<AsociadoEducacionDto> RegistrarProgresoAsync(Guid inscripcionId, int horasCursadas, string? resultado = null)
    {
        _logger.LogInformation(
            "Registrando progreso de inscripción {InscripcionId}: {Horas} horas cursadas",
            inscripcionId, horasCursadas);

        var existing = _inscripcionesStore.FirstOrDefault(i => i.Id == inscripcionId);
        if (existing == null)
        {
            throw new KeyNotFoundException($"Inscripción {inscripcionId} no encontrada");
        }

        var progreso = existing.HorasPrograma > 0
            ? Math.Round((decimal)horasCursadas / existing.HorasPrograma * 100, 2)
            : 0;

        var completado = horasCursadas >= existing.HorasPrograma;

        var updated = existing with
        {
            HorasCursadas = horasCursadas,
            Progreso = progreso,
            Completado = completado,
            FechaCompletado = completado ? DateTime.UtcNow : existing.FechaCompletado,
            Resultado = resultado ?? existing.Resultado
        };

        var index = _inscripcionesStore.IndexOf(existing);
        _inscripcionesStore[index] = updated;

        return Task.FromResult(updated);
    }

    /// <inheritdoc/>
    public Task<List<AsociadoEducacionDto>> GetHistorialAsync(Guid asociadoId)
    {
        _logger.LogInformation("Obteniendo historial educativo del asociado {AsociadoId}", asociadoId);

        var result = _inscripcionesStore
            .Where(i => i.AsociadoId == asociadoId)
            .ToList();

        return Task.FromResult(result);
    }

    /// <inheritdoc/>
    public Task<bool> CumpleMinimoHorasAsync(Guid asociadoId, int anio)
    {
        _logger.LogInformation(
            "Verificando horas mínimas para asociado {AsociadoId}, año {Anio}",
            asociadoId, anio);

        var totalHoras = _inscripcionesStore
            .Where(i => i.AsociadoId == asociadoId && i.Completado)
            .Sum(i => i.HorasCursadas);

        return Task.FromResult(totalHoras >= HorasMinimasAnuales);
    }
}
