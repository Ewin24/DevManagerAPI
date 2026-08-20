namespace Domain.Interfaces.Repositories;

using Domain.Entities.Bienestar;

/// <summary>
/// Repositorio de solicitudes de bienestar (single-entity: Domain IS the EF entity)
/// </summary>
public interface ISolicitudBienestarRepository
{
    Task<SolicitudBienestar> CreateAsync(SolicitudBienestar solicitud);

    Task<SolicitudBienestar?> GetByIdAsync(Guid id);

    Task<List<SolicitudBienestar>> GetByAsociadoAsync(Guid asociadoId);

    Task<SolicitudBienestar> UpdateAsync(SolicitudBienestar solicitud);
}