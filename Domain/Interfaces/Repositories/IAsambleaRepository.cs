namespace Domain.Interfaces.Repositories;

using Domain.Entities.Organos;

/// <summary>
/// Repositorio de asambleas generales (single-entity: Domain IS the EF entity)
/// </summary>
public interface IAsambleaRepository
{
    Task<Asamblea> CreateAsync(Asamblea asamblea);

    Task<Asamblea> UpdateAsync(Asamblea asamblea);

    Task<Asamblea?> GetByIdAsync(Guid id);

    Task<List<Asamblea>> GetByOrganizationAsync(Guid organizationId);
}