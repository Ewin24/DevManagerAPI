namespace Domain.Interfaces.Repositories;

using Domain.Entities.Nomina;

/// <summary>
/// Repositorio de compensaciones de asociados (single-entity: Domain IS the EF entity)
/// </summary>
public interface ICompensacionRepository
{
    Task<Compensacion> CreateAsync(Compensacion compensacion);

    Task<List<Compensacion>> GetByAsociadoAsync(Guid asociadoId, int anio);
}