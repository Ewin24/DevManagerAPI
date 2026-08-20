namespace Domain.Interfaces.Repositories;

using Domain.Entities.GestionHumana;

/// <summary>
/// Repositorio de inscripciones educativas de asociados (single-entity: Domain IS the EF entity)
/// </summary>
public interface IAsociadoEducacionRepository
{
    Task<AsociadoEducacion> CreateAsync(AsociadoEducacion inscripcion);

    Task<AsociadoEducacion?> GetByIdAsync(Guid id);

    Task<List<AsociadoEducacion>> GetByAsociadoAsync(Guid asociadoId);

    Task<AsociadoEducacion> UpdateAsync(AsociadoEducacion inscripcion);
}