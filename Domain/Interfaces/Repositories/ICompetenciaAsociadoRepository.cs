namespace Domain.Interfaces.Repositories;

using Domain.Entities.GestionHumana;

/// <summary>
/// Repositorio de competencias de asociados (single-entity: Domain IS the EF entity)
/// </summary>
public interface ICompetenciaAsociadoRepository
{
    Task<CompetenciaAsociado> CreateAsync(CompetenciaAsociado competencia);

    Task<CompetenciaAsociado?> GetByIdAsync(Guid id);

    Task<List<CompetenciaAsociado>> GetByAsociadoAsync(Guid asociadoId);

    Task<CompetenciaAsociado> UpdateAsync(CompetenciaAsociado competencia);

    Task<List<CompetenciaAsociado>> SearchByCompetenciaAsync(string competencia, bool soloDisponibles);
}