namespace Domain.Interfaces.Repositories;

using Domain.Entities.HabeasData;

/// <summary>
/// Repositorio de autorizaciones de tratamiento de datos (single-entity: Domain IS the EF entity)
/// </summary>
public interface IAutorizacionRepository
{
    Task<Autorizacion> CreateAsync(Autorizacion autorizacion);

    Task<Autorizacion> UpdateAsync(Autorizacion autorizacion);

    Task<Autorizacion?> GetByIdAsync(Guid id);

    Task<List<Autorizacion>> GetActiveByAsociadoAsync(Guid asociadoId);

    Task<Autorizacion?> GetVigenteByAsociadoAsync(Guid asociadoId);

    Task<bool> TieneVigenteAsync(Guid asociadoId);
}