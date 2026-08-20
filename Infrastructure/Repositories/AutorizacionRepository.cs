namespace Infrastructure.Repositories;

using Domain.Entities.HabeasData;
using Domain.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Implementación EF Core de <see cref="IAutorizacionRepository"/>
/// </summary>
public class AutorizacionRepository : IAutorizacionRepository
{
    private readonly DevManagerDbContext _context;

    public AutorizacionRepository(DevManagerDbContext context)
    {
        _context = context;
    }

    public async Task<Autorizacion> CreateAsync(Autorizacion autorizacion)
    {
        _context.Autorizaciones.Add(autorizacion);
        await _context.SaveChangesAsync();
        return autorizacion;
    }

    public async Task<Autorizacion> UpdateAsync(Autorizacion autorizacion)
    {
        var tracked = await _context.Autorizaciones
            .FirstOrDefaultAsync(a => a.Id == autorizacion.Id && !a.IsDeleted);
        if (tracked != null)
        {
            _context.Entry(tracked).CurrentValues.SetValues(autorizacion);
            await _context.SaveChangesAsync();
            return tracked;
        }
        return autorizacion;
    }

    public async Task<Autorizacion?> GetByIdAsync(Guid id)
    {
        return await _context.Autorizaciones
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
    }

    public async Task<List<Autorizacion>> GetActiveByAsociadoAsync(Guid asociadoId)
    {
        return await _context.Autorizaciones
            .AsNoTracking()
            .Where(a => a.AsociadoId == asociadoId && !a.Revocada && !a.IsDeleted)
            .ToListAsync();
    }

    public async Task<Autorizacion?> GetVigenteByAsociadoAsync(Guid asociadoId)
    {
        return await _context.Autorizaciones
            .AsNoTracking()
            .Where(a => a.AsociadoId == asociadoId
                        && !a.Revocada
                        && !a.IsDeleted
                        && (!a.Vigencia.HasValue || a.Vigencia >= DateTime.UtcNow))
            .OrderByDescending(a => a.FechaAutorizacion)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> TieneVigenteAsync(Guid asociadoId)
    {
        return await _context.Autorizaciones
            .AnyAsync(a => a.AsociadoId == asociadoId
                           && !a.Revocada
                           && !a.IsDeleted
                           && (!a.Vigencia.HasValue || a.Vigencia >= DateTime.UtcNow));
    }
}