namespace Domain.Interfaces.Repositories;

using Domain.Entities.SST;

/// <summary>
/// Repositorio de accidentes de trabajo (single-entity: Domain IS the EF entity)
/// </summary>
public interface IAccidenteRepository
{
    Task<Accidente> CreateAsync(Accidente accidente);

    Task<Accidente> UpdateAsync(Accidente accidente);

    Task<Accidente?> GetByIdAsync(Guid id);

    Task<List<Accidente>> GetPendientesInvestigacionAsync(Guid organizationId);

    Task<List<Accidente>> GetByOrganizationAsync(Guid organizationId);
}