namespace Domain.Interfaces.Repositories;

using Domain.Entities.BalanceSocial;

/// <summary>
/// Repositorio de indicadores de balance social (single-entity: Domain IS the EF entity)
/// </summary>
public interface IIndicadorBalanceSocialRepository
{
    Task<IndicadorBalanceSocial> CreateAsync(IndicadorBalanceSocial indicador);

    Task<IndicadorBalanceSocial> UpdateAsync(IndicadorBalanceSocial indicador);

    Task<IndicadorBalanceSocial?> GetByAsociadoAndAnioAsync(Guid asociadoId, int anio);

    Task<List<IndicadorBalanceSocial>> GetByOrganizationAndAnioAsync(Guid organizationId, int anio);
}