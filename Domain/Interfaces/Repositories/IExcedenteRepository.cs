namespace Domain.Interfaces.Repositories;

using Domain.Entities.Excedentes;

/// <summary>
/// Repositorio de distribuciones de excedentes (single-entity: Domain IS the EF entity)
/// </summary>
public interface IExcedenteRepository
{
    Task<Excedente> CreateAsync(Excedente excedente);

    Task<Excedente> UpdateAsync(Excedente excedente);

    Task<Excedente?> GetByIdAsync(Guid id);

    Task<Excedente?> GetByOrganizationAndPeriodoAsync(Guid organizationId, DateTime periodo);

    Task<List<Excedente>> GetByOrganizationAsync(Guid organizationId);
}