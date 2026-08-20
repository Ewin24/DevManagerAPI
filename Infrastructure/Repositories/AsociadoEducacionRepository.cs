namespace Infrastructure.Repositories;

using Domain.Entities.GestionHumana;
using Domain.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Implementación EF Core de <see cref="IAsociadoEducacionRepository"/>
/// </summary>
public class AsociadoEducacionRepository : IAsociadoEducacionRepository
{
    private readonly DevManagerDbContext _context;

    public AsociadoEducacionRepository(DevManagerDbContext context)
    {
        _context = context;
    }

    public async Task<AsociadoEducacion> CreateAsync(AsociadoEducacion inscripcion)
    {
        _context.AsociadosEducacion.Add(inscripcion);
        await _context.SaveChangesAsync();
        return inscripcion;
    }

    public async Task<AsociadoEducacion?> GetByIdAsync(Guid id)
    {
        return await _context.AsociadosEducacion
            .AsNoTracking()
            .Include(i => i.Programa)
            .FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);
    }

    public async Task<List<AsociadoEducacion>> GetByAsociadoAsync(Guid asociadoId)
    {
        return await _context.AsociadosEducacion
            .AsNoTracking()
            .Include(i => i.Programa)
            .Where(i => i.AsociadoId == asociadoId && !i.IsDeleted)
            .OrderByDescending(i => i.FechaInscripcion)
            .ToListAsync();
    }

    public async Task<AsociadoEducacion> UpdateAsync(AsociadoEducacion inscripcion)
    {
        var tracked = await _context.AsociadosEducacion
            .FirstOrDefaultAsync(i => i.Id == inscripcion.Id && !i.IsDeleted);
        if (tracked != null)
        {
            _context.Entry(tracked).CurrentValues.SetValues(inscripcion);
            await _context.SaveChangesAsync();
            return tracked;
        }
        return inscripcion;
    }
}