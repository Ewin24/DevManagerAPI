namespace Domain.Interfaces.Repositories;

using Domain.Entities.Bienestar;

/// <summary>
/// Repositorio de programas de bienestar (single-entity: Domain IS the EF entity)
/// </summary>
public interface IProgramaBienestarRepository
{
    Task<ProgramaBienestar> CreateAsync(ProgramaBienestar programa);

    Task<ProgramaBienestar?> GetByIdAsync(Guid id);

    Task<List<ProgramaBienestar>> GetByOrganizationAsync(Guid organizationId);
}