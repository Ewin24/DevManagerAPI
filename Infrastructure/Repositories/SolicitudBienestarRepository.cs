namespace Infrastructure.Repositories;

using Domain.Entities.Bienestar;
using Domain.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Implementación EF Core de <see cref="ISolicitudBienestarRepository"/>
/// </summary>
public class SolicitudBienestarRepository : ISolicitudBienestarRepository
{
    private readonly DevManagerDbContext _context;

    public SolicitudBienestarRepository(DevManagerDbContext context)
    {
        _context = context;
    }

    public async Task<SolicitudBienestar> CreateAsync(SolicitudBienestar solicitud)
    {
        _context.SolicitudesBienestar.Add(solicitud);
        await _context.SaveChangesAsync();
        return solicitud;
    }

    public async Task<SolicitudBienestar?> GetByIdAsync(Guid id)
    {
        return await _context.SolicitudesBienestar
            .AsNoTracking()
            .Include(s => s.Programa)
            .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
    }

    public async Task<List<SolicitudBienestar>> GetByAsociadoAsync(Guid asociadoId)
    {
        return await _context.SolicitudesBienestar
            .AsNoTracking()
            .Include(s => s.Programa)
            .Where(s => s.AsociadoId == asociadoId && !s.IsDeleted)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<SolicitudBienestar> UpdateAsync(SolicitudBienestar solicitud)
    {
        var tracked = await _context.SolicitudesBienestar
            .FirstOrDefaultAsync(s => s.Id == solicitud.Id && !s.IsDeleted);
        if (tracked != null)
        {
            _context.Entry(tracked).CurrentValues.SetValues(solicitud);
            await _context.SaveChangesAsync();
            return tracked;
        }
        return solicitud;
    }
}