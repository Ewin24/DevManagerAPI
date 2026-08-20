namespace Application.Services.GestionHumana;

using Application.DTOs.GestionHumana;
using Application.Interfaces;
using Domain.Entities.GestionHumana;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

/// <summary>
/// Implementación del servicio de educación cooperativa
/// Gestión de programas, inscripciones y cumplimiento de horas mínimas
/// Mínimo 20 horas anuales según art.88-91 normativa solidaria
/// </summary>
public class EducacionService : IEducacionService
{
    private readonly IProgramaEducacionRepository _programasRepository;
    private readonly IAsociadoEducacionRepository _inscripcionesRepository;
    private readonly ILogger<EducacionService> _logger;

    private const int HorasMinimasAnuales = 20;

    public EducacionService(
        IProgramaEducacionRepository programasRepository,
        IAsociadoEducacionRepository inscripcionesRepository,
        ILogger<EducacionService> logger)
    {
        _programasRepository = programasRepository;
        _inscripcionesRepository = inscripcionesRepository;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<List<ProgramaEducacionDto>> GetProgramasAsync(Guid organizationId)
    {
        _logger.LogInformation("Obteniendo programas educativos de organización {OrgId}", organizationId);

        var programas = await _programasRepository.GetByOrganizationAsync(organizationId);
        return programas.Select(MapToDto).ToList();
    }

    /// <inheritdoc/>
    public async Task<ProgramaEducacionDto> CreateProgramaAsync(CreateProgramaEducacionDto dto)
    {
        _logger.LogInformation(
            "Creando programa educativo '{Nombre}', tipo {Tipo}, {Horas} horas",
            dto.Nombre, dto.Tipo, dto.Horas);

        var programa = new ProgramaEducacion
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

        var creado = await _programasRepository.CreateAsync(programa);
        return MapToDto(creado);
    }

    /// <inheritdoc/>
    public async Task<AsociadoEducacionDto> InscribirAsync(CreateAsociadoEducacionDto dto)
    {
        _logger.LogInformation(
            "Inscribiendo asociado {AsociadoId} en programa {ProgramaId}",
            dto.AsociadoId, dto.ProgramaEducacionId);

        var programa = await _programasRepository.GetByIdAsync(dto.ProgramaEducacionId);

        var inscripcion = new AsociadoEducacion
        {
            Id = Guid.NewGuid(),
            AsociadoId = dto.AsociadoId,
            ProgramaEducacionId = dto.ProgramaEducacionId,
            OrganizationId = dto.OrganizationId,
            HorasCursadas = 0,
            Progreso = 0,
            FechaInscripcion = DateTime.UtcNow,
            Completado = false,
            CreatedAt = DateTime.UtcNow
        };

        var creada = await _inscripcionesRepository.CreateAsync(inscripcion);
        return MapToDto(creada, programa);
    }

    /// <inheritdoc/>
    public async Task<AsociadoEducacionDto> RegistrarProgresoAsync(Guid inscripcionId, int horasCursadas, string? resultado = null)
    {
        _logger.LogInformation(
            "Registrando progreso de inscripción {InscripcionId}: {Horas} horas cursadas",
            inscripcionId, horasCursadas);

        var existing = await _inscripcionesRepository.GetByIdAsync(inscripcionId);
        if (existing == null)
        {
            throw new KeyNotFoundException($"Inscripción {inscripcionId} no encontrada");
        }

        var programa = existing.Programa;
        var horasPrograma = programa?.Horas ?? 0;
        var progreso = horasPrograma > 0
            ? Math.Round((decimal)horasCursadas / horasPrograma * 100, 2)
            : 0;

        var completado = horasCursadas >= horasPrograma;

        existing.HorasCursadas = horasCursadas;
        existing.Progreso = progreso;
        existing.Completado = completado;
        existing.FechaCompletado = completado ? DateTime.UtcNow : existing.FechaCompletado;
        existing.Resultado = resultado ?? existing.Resultado;

        var updated = await _inscripcionesRepository.UpdateAsync(existing);
        return MapToDto(updated, programa);
    }

    /// <inheritdoc/>
    public async Task<List<AsociadoEducacionDto>> GetHistorialAsync(Guid asociadoId)
    {
        _logger.LogInformation("Obteniendo historial educativo del asociado {AsociadoId}", asociadoId);

        var inscripciones = await _inscripcionesRepository.GetByAsociadoAsync(asociadoId);
        return inscripciones.Select(i => MapToDto(i, i.Programa)).ToList();
    }

    /// <inheritdoc/>
    public async Task<bool> CumpleMinimoHorasAsync(Guid asociadoId, int anio)
    {
        _logger.LogInformation(
            "Verificando horas mínimas para asociado {AsociadoId}, año {Anio}",
            asociadoId, anio);

        var inscripciones = await _inscripcionesRepository.GetByAsociadoAsync(asociadoId);
        var totalHoras = inscripciones
            .Where(i => i.Completado)
            .Sum(i => i.HorasCursadas);

        return totalHoras >= HorasMinimasAnuales;
    }

    // ===== Mapping =====

    private static ProgramaEducacionDto MapToDto(ProgramaEducacion p) => new()
    {
        Id = p.Id,
        OrganizationId = p.OrganizationId,
        Nombre = p.Nombre,
        Descripcion = p.Descripcion,
        Tipo = p.Tipo,
        Horas = p.Horas,
        EsObligatorio = p.EsObligatorio,
        FechaInicio = p.FechaInicio,
        FechaFin = p.FechaFin,
        Activo = p.Activo,
        CreatedAt = p.CreatedAt
    };

    private static AsociadoEducacionDto MapToDto(AsociadoEducacion i, ProgramaEducacion? programa) => new()
    {
        Id = i.Id,
        AsociadoId = i.AsociadoId,
        ProgramaEducacionId = i.ProgramaEducacionId,
        ProgramaNombre = programa?.Nombre,
        TipoEducacion = programa?.Tipo.ToString(),
        HorasPrograma = programa?.Horas ?? 0,
        HorasCursadas = i.HorasCursadas,
        Progreso = i.Progreso,
        FechaInscripcion = i.FechaInscripcion,
        FechaCompletado = i.FechaCompletado,
        Completado = i.Completado,
        Resultado = i.Resultado
    };
}