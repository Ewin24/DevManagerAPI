namespace Domain.Interfaces.Repositories;

using Domain.Entities.Organos;

/// <summary>
/// Repositorio de miembros de órganos (single-entity: Domain IS the EF entity)
/// </summary>
public interface IMiembroOrganoRepository
{
    Task<MiembroOrgano> CreateAsync(MiembroOrgano miembro);

    Task<MiembroOrgano> UpdateAsync(MiembroOrgano miembro);

    Task<MiembroOrgano?> GetByIdAsync(Guid id);

    Task<List<MiembroOrgano>> GetByOrganoAsync(Guid organoId);

    Task<int> CountByOrganoAsync(Guid organoId);

    Task<bool> DeleteAsync(Guid id);
}