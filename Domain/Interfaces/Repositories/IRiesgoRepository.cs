namespace Domain.Interfaces.Repositories;

using Domain.Entities.SST;

/// <summary>
/// Repositorio de la matriz de riesgos laborales (single-entity: Domain IS the EF entity)
/// </summary>
public interface IRiesgoRepository
{
    Task<Riesgo> CreateAsync(Riesgo riesgo);

    Task<List<Riesgo>> GetByOrganizationAsync(Guid organizationId);
}