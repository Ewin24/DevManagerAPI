namespace Infrastructure.Repositories;

using Domain.Entities.Organos;
using Domain.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Implementación EF Core de <see cref="IVotoRepository"/>
/// </summary>
public class VotoRepository : IVotoRepository
{
    private readonly DevManagerDbContext _context;

    public VotoRepository(DevManagerDbContext context)
    {
        _context = context;
    }

    public async Task<Voto> CreateAsync(Voto voto)
    {
        _context.Votos.Add(voto);
        await _context.SaveChangesAsync();
        return voto;
    }

    public async Task<List<Voto>> GetByAsambleaAsync(Guid asambleaId)
    {
        return await _context.Votos
            .AsNoTracking()
            .Where(v => v.AsambleaId == asambleaId && !v.IsDeleted)
            .ToListAsync();
    }

    public async Task<int> CountByAsambleaAsync(Guid asambleaId)
    {
        return await _context.Votos
            .CountAsync(v => v.AsambleaId == asambleaId && !v.IsDeleted);
    }

    public async Task<bool> ExistsByAsambleaAndAsociadoAsync(Guid asambleaId, Guid asociadoId)
    {
        return await _context.Votos
            .AnyAsync(v => v.AsambleaId == asambleaId && v.AsociadoId == asociadoId && !v.IsDeleted);
    }
}