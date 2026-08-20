namespace Domain.Interfaces.Repositories;

using Domain.Entities.Organos;

/// <summary>
/// Repositorio de votos en asambleas (single-entity: Domain IS the EF entity)
/// </summary>
public interface IVotoRepository
{
    Task<Voto> CreateAsync(Voto voto);

    Task<List<Voto>> GetByAsambleaAsync(Guid asambleaId);

    Task<int> CountByAsambleaAsync(Guid asambleaId);

    Task<bool> ExistsByAsambleaAndAsociadoAsync(Guid asambleaId, Guid asociadoId);
}