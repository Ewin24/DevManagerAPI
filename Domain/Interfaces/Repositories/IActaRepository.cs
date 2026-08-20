namespace Domain.Interfaces.Repositories;

using Domain.Entities.Organos;

/// <summary>
/// Repositorio de actas de órganos (single-entity: Domain IS the EF entity)
/// </summary>
public interface IActaRepository
{
    Task<Acta> CreateAsync(Acta acta);

    Task<Acta?> GetByIdAsync(Guid id);

    Task<List<Acta>> GetByOrganoAsync(Guid organoId);

    Task<int> CountByOrganoAsync(Guid organoId);
}