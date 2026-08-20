namespace Infrastructure.Repositories;

using Domain.Entities.SST;
using Domain.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Implementación EF Core de <see cref="IRiesgoRepository"/>
/// </summary>
public class RiesgoRepository : IRiesgoRepository
{
    private readonly DevManagerDbContext _context;

    public RiesgoRepository(DevManagerDbContext context)
    {
        _context = context;
    }

    public async Task<Riesgo> CreateAsync(Riesgo riesgo)
    {
        _context.Riesgos.Add(riesgo);
        await _context.SaveChangesAsync();
        return riesgo;
    }

    public async Task<List<Riesgo>> GetByOrganizationAsync(Guid organizationId)
    {
        return await _context.Riesgos
            .AsNoTracking()
            .Where(r => r.OrganizationId == organizationId && !r.IsDeleted && r.Activo)
            .ToListAsync();
    }
}