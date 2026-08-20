namespace Infrastructure.Repositories;

using Domain.Entities.Organos;
using Domain.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Implementación EF Core de <see cref="IMiembroOrganoRepository"/>
/// </summary>
public class MiembroOrganoRepository : IMiembroOrganoRepository
{
    private readonly DevManagerDbContext _context;

    public MiembroOrganoRepository(DevManagerDbContext context)
    {
        _context = context;
    }

    public async Task<MiembroOrgano> CreateAsync(MiembroOrgano miembro)
    {
        _context.MiembrosOrgano.Add(miembro);
        await _context.SaveChangesAsync();
        return miembro;
    }

    public async Task<MiembroOrgano> UpdateAsync(MiembroOrgano miembro)
    {
        var tracked = await _context.MiembrosOrgano
            .FirstOrDefaultAsync(m => m.Id == miembro.Id && !m.IsDeleted);
        if (tracked != null)
        {
            _context.Entry(tracked).CurrentValues.SetValues(miembro);
            await _context.SaveChangesAsync();
            return tracked;
        }
        return miembro;
    }

    public async Task<MiembroOrgano?> GetByIdAsync(Guid id)
    {
        return await _context.MiembrosOrgano
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted);
    }

    public async Task<List<MiembroOrgano>> GetByOrganoAsync(Guid organoId)
    {
        return await _context.MiembrosOrgano
            .AsNoTracking()
            .Where(m => m.OrganoId == organoId && !m.IsDeleted)
            .ToListAsync();
    }

    public async Task<int> CountByOrganoAsync(Guid organoId)
    {
        return await _context.MiembrosOrgano
            .CountAsync(m => m.OrganoId == organoId && !m.IsDeleted);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var miembro = await _context.MiembrosOrgano
            .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted);
        if (miembro == null)
            return false;

        miembro.IsDeleted = true;
        miembro.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }
}