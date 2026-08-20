namespace Infrastructure.Repositories;

using Domain.Entities.Organos;
using Domain.Interfaces.Repositories;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Implementación EF Core de <see cref="IOrganoRepository"/>
/// </summary>
public class OrganoRepository : IOrganoRepository
{
    private readonly DevManagerDbContext _context;

    public OrganoRepository(DevManagerDbContext context)
    {
        _context = context;
    }

    public async Task<Organo> CreateAsync(Organo organo)
    {
        _context.Organos.Add(organo);
        await _context.SaveChangesAsync();
        return organo;
    }

    public async Task<Organo> UpdateAsync(Organo organo)
    {
        var tracked = await _context.Organos
            .FirstOrDefaultAsync(o => o.Id == organo.Id && !o.IsDeleted);
        if (tracked != null)
        {
            _context.Entry(tracked).CurrentValues.SetValues(organo);
            await _context.SaveChangesAsync();
            return tracked;
        }
        return organo;
    }

    public async Task<Organo?> GetByIdAsync(Guid id)
    {
        return await _context.Organos
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id && !o.IsDeleted);
    }

    public async Task<List<Organo>> GetByOrganizationAsync(Guid organizationId)
    {
        return await _context.Organos
            .AsNoTracking()
            .Where(o => o.OrganizationId == organizationId && !o.IsDeleted)
            .ToListAsync();
    }

    public async Task<List<Organo>> GetByTypeAsync(Guid organizationId, TipoOrgano tipo)
    {
        return await _context.Organos
            .AsNoTracking()
            .Where(o => o.OrganizationId == organizationId && o.Tipo == tipo && !o.IsDeleted)
            .ToListAsync();
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var organo = await _context.Organos
            .FirstOrDefaultAsync(o => o.Id == id && !o.IsDeleted);
        if (organo == null)
            return false;

        organo.IsDeleted = true;
        organo.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }
}