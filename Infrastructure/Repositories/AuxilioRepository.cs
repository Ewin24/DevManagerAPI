namespace Infrastructure.Repositories;

using Domain.Entities.Bienestar;
using Domain.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Implementación EF Core de <see cref="IAuxilioRepository"/>
/// </summary>
public class AuxilioRepository : IAuxilioRepository
{
    private readonly DevManagerDbContext _context;

    public AuxilioRepository(DevManagerDbContext context)
    {
        _context = context;
    }

    public async Task<Auxilio> CreateAsync(Auxilio auxilio)
    {
        _context.Auxilios.Add(auxilio);
        await _context.SaveChangesAsync();
        return auxilio;
    }

    public async Task<List<Auxilio>> GetByAsociadoAsync(Guid asociadoId)
    {
        return await _context.Auxilios
            .AsNoTracking()
            .Where(a => a.AsociadoId == asociadoId && !a.IsDeleted)
            .OrderByDescending(a => a.FechaEntrega)
            .ToListAsync();
    }
}