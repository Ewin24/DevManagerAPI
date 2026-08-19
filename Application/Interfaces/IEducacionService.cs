namespace Application.Interfaces;

using Application.DTOs.GestionHumana;

/// <summary>
/// Servicio de educación cooperativa — programas, inscripciones y
/// cumplimiento de horas mínimas (20hr art.88-91)
/// </summary>
public interface IEducacionService
{
    /// <summary>Obtiene todos los programas educativos activos</summary>
    Task<List<ProgramaEducacionDto>> GetProgramasAsync(Guid organizationId);

    /// <summary>Crea un nuevo programa educativo</summary>
    Task<ProgramaEducacionDto> CreateProgramaAsync(CreateProgramaEducacionDto dto);

    /// <summary>Inscribe un asociado a un programa educativo</summary>
    Task<AsociadoEducacionDto> InscribirAsync(CreateAsociadoEducacionDto dto);

    /// <summary>Registra progreso educativo de un asociado</summary>
    Task<AsociadoEducacionDto> RegistrarProgresoAsync(Guid inscripcionId, int horasCursadas, string? resultado = null);

    /// <summary>Obtiene el historial educativo de un asociado</summary>
    Task<List<AsociadoEducacionDto>> GetHistorialAsync(Guid asociadoId);

    /// <summary>Verifica si un asociado cumple con las 20hr mínimas anuales</summary>
    Task<bool> CumpleMinimoHorasAsync(Guid asociadoId, int anio);
}
