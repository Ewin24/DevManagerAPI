namespace Infrastructure.Repositories;

using Domain.Entities.Organos;
using Domain.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Implementación EF Core de <see cref="IAsambleaRepository"/>
/// </summary>
public class AsambleaRepository : IAsambleaRepository
{
    private readonly DevManagerDbContext _context;

    public AsambleaRepository(DevManagerDbContext context)
    {
        _context = context;
    }

    public async Task<Asamblea> CreateAsync(Asamblea asamblea)
    {
        _context.Asambleas.Add(asamblea);
        await _context.SaveChangesAsync();
        return asamblea;
    }

    public async Task<Asamblea> UpdateAsync(Asamblea asamblea)
    {
        var tracked = await _context.Asambleas
            .FirstOrDefaultAsync(a => a.Id == asamblea.Id && !a.IsDeleted);
        if (tracked != null)
        {
            _context.Entry(tracked).CurrentValues.SetValues(asamblea);
            await _context.SaveChangesAsync();
            return tracked;
        }
        return asamblea;
    }

    public async Task<Asamblea?> GetByIdAsync(Guid id)
    {
        return await _context.Asambleas
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
    }

    public async Task<List<Asamblea>> GetByOrganizationAsync(Guid organizationId)
    {
        return await _context.Asambleas
            .AsNoTracking()
            .Where(a => a.OrganizationId == organizationId && !a.IsDeleted)
            .OrderByDescending(a => a.Fecha)
            .ToListAsync();
    }
}