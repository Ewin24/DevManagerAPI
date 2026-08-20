namespace Domain.Interfaces.Repositories;

using Domain.Entities.Nomina;

/// <summary>
/// Repositorio de aportes PILA (single-entity: Domain IS the EF entity)
/// </summary>
public interface IPilaAporteRepository
{
    Task<PilaAporte> CreateAsync(PilaAporte aporte);

    Task<List<PilaAporte>> GetByOrganizationAndPeriodoAsync(Guid organizationId, int mes, int anio);
}