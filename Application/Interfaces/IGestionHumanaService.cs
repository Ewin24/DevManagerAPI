namespace Application.Interfaces;

using Application.DTOs.GestionHumana;

/// <summary>
/// Servicio de gestión humana solidaria — administración de asociados,
/// competencias cooperativas y perfiles con dimensión solidaria
/// </summary>
public interface IGestionHumanaService
{
    /// <summary>Obtiene las competencias de un asociado</summary>
    Task<List<CompetenciaAsociadoDto>> GetCompetenciasAsync(Guid asociadoId);

    /// <summary>Registra una competencia para un asociado</summary>
    Task<CompetenciaAsociadoDto> CreateCompetenciaAsync(Guid asociadoId, Guid organizationId, string competencia, int nivel);

    /// <summary>Actualiza la disponibilidad de una competencia</summary>
    Task<CompetenciaAsociadoDto> UpdateDisponibilidadAsync(Guid competenciaId, bool disponible);

    /// <summary>Busca asociados por competencia para servicio solidario</summary>
    Task<List<CompetenciaAsociadoDto>> BuscarPorCompetenciaAsync(string competencia, bool soloDisponibles = true);
}
