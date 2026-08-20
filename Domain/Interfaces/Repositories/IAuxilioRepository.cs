namespace Domain.Interfaces.Repositories;

using Domain.Entities.Bienestar;

/// <summary>
/// Repositorio de auxilios entregados (single-entity: Domain IS the EF entity)
/// </summary>
public interface IAuxilioRepository
{
    Task<Auxilio> CreateAsync(Auxilio auxilio);

    Task<List<Auxilio>> GetByAsociadoAsync(Guid asociadoId);
}