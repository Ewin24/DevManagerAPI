namespace Infrastructure.Repositories;

using Domain.Entities.GestionHumana;
using Domain.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Implementación EF Core de <see cref="ICompetenciaAsociadoRepository"/>
/// </summary>
public class CompetenciaAsociadoRepository : ICompetenciaAsociadoRepository
{
    private readonly DevManagerDbContext _context;

    public CompetenciaAsociadoRepository(DevManagerDbContext context)
    {
        _context = context;
    }

    public async Task<CompetenciaAsociado> CreateAsync(CompetenciaAsociado competencia)
    {
        _context.CompetenciasAsociado.Add(competencia);
        await _context.SaveChangesAsync();
        return competencia;
    }

    public async Task<CompetenciaAsociado?> GetByIdAsync(Guid id)
    {
        return await _context.CompetenciasAsociado
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
    }

    public async Task<List<CompetenciaAsociado>> GetByAsociadoAsync(Guid asociadoId)
    {
        return await _context.CompetenciasAsociado
            .AsNoTracking()
            .Where(c => c.AsociadoId == asociadoId && !c.IsDeleted)
            .OrderByDescending(c => c.FechaActualizacion)
            .ToListAsync();
    }

    public async Task<CompetenciaAsociado> UpdateAsync(CompetenciaAsociado competencia)
    {
        var tracked = await _context.CompetenciasAsociado
            .FirstOrDefaultAsync(c => c.Id == competencia.Id && !c.IsDeleted);
        if (tracked != null)
        {
            _context.Entry(tracked).CurrentValues.SetValues(competencia);
            await _context.SaveChangesAsync();
            return tracked;
        }
        return competencia;
    }

    public async Task<List<CompetenciaAsociado>> SearchByCompetenciaAsync(string competencia, bool soloDisponibles)
    {
        var query = _context.CompetenciasAsociado
            .AsNoTracking()
            .Where(c => !c.IsDeleted && c.Competencia.Contains(competencia));

        if (soloDisponibles)
        {
            query = query.Where(c => c.Disponible);
        }

        return await query.ToListAsync();
    }
}