namespace Domain.Interfaces.Repositories;

using Domain.Entities.GestionHumana;

/// <summary>
/// Repositorio de programas educativos (single-entity: Domain IS the EF entity)
/// </summary>
public interface IProgramaEducacionRepository
{
    Task<ProgramaEducacion> CreateAsync(ProgramaEducacion programa);

    Task<ProgramaEducacion?> GetByIdAsync(Guid id);

    Task<List<ProgramaEducacion>> GetByOrganizationAsync(Guid organizationId);
}