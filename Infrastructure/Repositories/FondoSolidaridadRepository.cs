namespace Infrastructure.Repositories;

using Domain.Entities.Bienestar;
using Domain.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Implementación EF Core de <see cref="IFondoSolidaridadRepository"/>
/// </summary>
public class FondoSolidaridadRepository : IFondoSolidaridadRepository
{
    private readonly DevManagerDbContext _context;

    public FondoSolidaridadRepository(DevManagerDbContext context)
    {
        _context = context;
    }

    public async Task<FondoSolidaridad> CreateAsync(FondoSolidaridad fondo)
    {
        _context.FondosSolidaridad.Add(fondo);
        await _context.SaveChangesAsync();
        return fondo;
    }

    public async Task<FondoSolidaridad> UpdateAsync(FondoSolidaridad fondo)
    {
        var tracked = await _context.FondosSolidaridad
            .FirstOrDefaultAsync(f => f.Id == fondo.Id && !f.IsDeleted);
        if (tracked != null)
        {
            _context.Entry(tracked).CurrentValues.SetValues(fondo);
            await _context.SaveChangesAsync();
            return tracked;
        }
        return fondo;
    }

    public async Task<FondoSolidaridad?> GetByOrganizationAndPeriodoAsync(Guid organizationId, DateTime periodo)
    {
        return await _context.FondosSolidaridad
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.OrganizationId == organizationId && f.Periodo == periodo && !f.IsDeleted);
    }

    public async Task<FondoSolidaridad?> GetActualAsync(Guid organizationId)
    {
        return await _context.FondosSolidaridad
            .AsNoTracking()
            .Where(f => f.OrganizationId == organizationId && f.Vigente && !f.IsDeleted)
            .OrderByDescending(f => f.Periodo)
            .FirstOrDefaultAsync();
    }
}