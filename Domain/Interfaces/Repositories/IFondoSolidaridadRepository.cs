namespace Domain.Interfaces.Repositories;

using Domain.Entities.Bienestar;

/// <summary>
/// Repositorio del fondo de solidaridad (single-entity: Domain IS the EF entity)
/// </summary>
public interface IFondoSolidaridadRepository
{
    Task<FondoSolidaridad> CreateAsync(FondoSolidaridad fondo);

    Task<FondoSolidaridad> UpdateAsync(FondoSolidaridad fondo);

    Task<FondoSolidaridad?> GetByOrganizationAndPeriodoAsync(Guid organizationId, DateTime periodo);

    Task<FondoSolidaridad?> GetActualAsync(Guid organizationId);
}