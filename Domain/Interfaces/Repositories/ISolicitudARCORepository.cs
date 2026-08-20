namespace Domain.Interfaces.Repositories;

using Domain.Entities.HabeasData;

/// <summary>
/// Repositorio de solicitudes ARCO (single-entity: Domain IS the EF entity)
/// </summary>
public interface ISolicitudARCORepository
{
    Task<SolicitudARCO> CreateAsync(SolicitudARCO solicitud);

    Task<SolicitudARCO> UpdateAsync(SolicitudARCO solicitud);

    Task<SolicitudARCO?> GetByIdAsync(Guid id);

    Task<List<SolicitudARCO>> GetByAsociadoAsync(Guid asociadoId);

    Task<List<SolicitudARCO>> GetPendientesAsync(Guid organizationId);
}