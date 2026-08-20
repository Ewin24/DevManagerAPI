namespace Domain.Interfaces.Repositories;

using Domain.Entities.Organos;
using Domain.Enums;

/// <summary>
/// Repositorio de órganos de administración (single-entity: Domain IS the EF entity)
/// </summary>
public interface IOrganoRepository
{
    Task<Organo> CreateAsync(Organo organo);

    Task<Organo> UpdateAsync(Organo organo);

    Task<Organo?> GetByIdAsync(Guid id);

    Task<List<Organo>> GetByOrganizationAsync(Guid organizationId);

    Task<List<Organo>> GetByTypeAsync(Guid organizationId, TipoOrgano tipo);

    Task<bool> DeleteAsync(Guid id);
}