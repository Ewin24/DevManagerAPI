namespace Infrastructure.Repositories;

using Domain.Entities.HabeasData;
using Domain.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Implementación EF Core de <see cref="ISolicitudARCORepository"/>
/// </summary>
public class SolicitudARCORepository : ISolicitudARCORepository
{
    private readonly DevManagerDbContext _context;

    public SolicitudARCORepository(DevManagerDbContext context)
    {
        _context = context;
    }

    public async Task<SolicitudARCO> CreateAsync(SolicitudARCO solicitud)
    {
        _context.SolicitudesARCO.Add(solicitud);
        await _context.SaveChangesAsync();
        return solicitud;
    }

    public async Task<SolicitudARCO> UpdateAsync(SolicitudARCO solicitud)
    {
        var tracked = await _context.SolicitudesARCO
            .FirstOrDefaultAsync(s => s.Id == solicitud.Id && !s.IsDeleted);
        if (tracked != null)
        {
            _context.Entry(tracked).CurrentValues.SetValues(solicitud);
            await _context.SaveChangesAsync();
            return tracked;
        }
        return solicitud;
    }

    public async Task<SolicitudARCO?> GetByIdAsync(Guid id)
    {
        return await _context.SolicitudesARCO
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
    }

    public async Task<List<SolicitudARCO>> GetByAsociadoAsync(Guid asociadoId)
    {
        return await _context.SolicitudesARCO
            .AsNoTracking()
            .Where(s => s.AsociadoId == asociadoId && !s.IsDeleted)
            .OrderByDescending(s => s.Fecha)
            .ToListAsync();
    }

    public async Task<List<SolicitudARCO>> GetPendientesAsync(Guid organizationId)
    {
        return await _context.SolicitudesARCO
            .AsNoTracking()
            .Where(s => s.OrganizationId == organizationId && s.Estado == Domain.Enums.EstadoSolicitudARCO.Pendiente && !s.IsDeleted)
            .OrderByDescending(s => s.Fecha)
            .ToListAsync();
    }
}