namespace Infrastructure.Repositories;

using Domain.Entities.Organos;
using Domain.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Implementación EF Core de <see cref="IActaRepository"/>
/// </summary>
public class ActaRepository : IActaRepository
{
    private readonly DevManagerDbContext _context;

    public ActaRepository(DevManagerDbContext context)
    {
        _context = context;
    }

    public async Task<Acta> CreateAsync(Acta acta)
    {
        _context.Actas.Add(acta);
        await _context.SaveChangesAsync();
        return acta;
    }

    public async Task<Acta?> GetByIdAsync(Guid id)
    {
        return await _context.Actas
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
    }

    public async Task<List<Acta>> GetByOrganoAsync(Guid organoId)
    {
        return await _context.Actas
            .AsNoTracking()
            .Where(a => a.OrganoId == organoId && !a.IsDeleted)
            .OrderByDescending(a => a.Fecha)
            .ToListAsync();
    }

    public async Task<int> CountByOrganoAsync(Guid organoId)
    {
        return await _context.Actas
            .CountAsync(a => a.OrganoId == organoId && !a.IsDeleted);
    }
}