namespace Domain.Interfaces.Repositories;

using Domain.Entities.SST;

/// <summary>
/// Repositorio de exámenes médicos ocupacionales (single-entity: Domain IS the EF entity)
/// </summary>
public interface IExamenMedicoRepository
{
    Task<ExamenMedico> CreateAsync(ExamenMedico examen);

    Task<ExamenMedico> UpdateAsync(ExamenMedico examen);

    Task<ExamenMedico?> GetByIdAsync(Guid id);

    Task<List<ExamenMedico>> GetByAsociadoAsync(Guid asociadoId);

    Task<List<ExamenMedico>> GetPendientesAsync(Guid organizationId);
}