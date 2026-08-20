namespace Infrastructure.Repositories;

using Domain.Entities.Bienestar;
using Domain.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Implementación EF Core de <see cref="IProgramaBienestarRepository"/>
/// </summary>
public class ProgramaBienestarRepository : IProgramaBienestarRepository
{
    private readonly DevManagerDbContext _context;

    public ProgramaBienestarRepository(DevManagerDbContext context)
    {
        _context = context;
    }

    public async Task<ProgramaBienestar> CreateAsync(ProgramaBienestar programa)
    {
        _context.ProgramasBienestar.Add(programa);
        await _context.SaveChangesAsync();
        return programa;
    }

    public async Task<ProgramaBienestar?> GetByIdAsync(Guid id)
    {
        return await _context.ProgramasBienestar
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
    }

    public async Task<List<ProgramaBienestar>> GetByOrganizationAsync(Guid organizationId)
    {
        return await _context.ProgramasBienestar
            .AsNoTracking()
            .Where(p => p.OrganizationId == organizationId && !p.IsDeleted)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }
}