namespace Infrastructure.Repositories;

using Domain.Entities.SST;
using Domain.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Implementación EF Core de <see cref="IAccidenteRepository"/>
/// </summary>
public class AccidenteRepository : IAccidenteRepository
{
    private readonly DevManagerDbContext _context;

    public AccidenteRepository(DevManagerDbContext context)
    {
        _context = context;
    }

    public async Task<Accidente> CreateAsync(Accidente accidente)
    {
        _context.Accidentes.Add(accidente);
        await _context.SaveChangesAsync();
        return accidente;
    }

    public async Task<Accidente> UpdateAsync(Accidente accidente)
    {
        var tracked = await _context.Accidentes
            .FirstOrDefaultAsync(a => a.Id == accidente.Id && !a.IsDeleted);
        if (tracked != null)
        {
            _context.Entry(tracked).CurrentValues.SetValues(accidente);
            await _context.SaveChangesAsync();
            return tracked;
        }
        return accidente;
    }

    public async Task<Accidente?> GetByIdAsync(Guid id)
    {
        return await _context.Accidentes
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
    }

    public async Task<List<Accidente>> GetPendientesInvestigacionAsync(Guid organizationId)
    {
        return await _context.Accidentes
            .AsNoTracking()
            .Where(a => a.OrganizationId == organizationId && !a.IsDeleted && !a.InvestigacionCompletada)
            .OrderByDescending(a => a.Fecha)
            .ToListAsync();
    }

    public async Task<List<Accidente>> GetByOrganizationAsync(Guid organizationId)
    {
        return await _context.Accidentes
            .AsNoTracking()
            .Where(a => a.OrganizationId == organizationId && !a.IsDeleted)
            .OrderByDescending(a => a.Fecha)
            .ToListAsync();
    }
}