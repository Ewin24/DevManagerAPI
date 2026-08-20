namespace Infrastructure.Repositories;

using Domain.Entities.Excedentes;
using Domain.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Implementación EF Core de <see cref="IExcedenteRepository"/>
/// </summary>
public class ExcedenteRepository : IExcedenteRepository
{
    private readonly DevManagerDbContext _context;

    public ExcedenteRepository(DevManagerDbContext context)
    {
        _context = context;
    }

    public async Task<Excedente> CreateAsync(Excedente excedente)
    {
        _context.Excedentes.Add(excedente);
        await _context.SaveChangesAsync();
        return excedente;
    }

    public async Task<Excedente> UpdateAsync(Excedente excedente)
    {
        var tracked = await _context.Excedentes
            .FirstOrDefaultAsync(e => e.Id == excedente.Id && !e.IsDeleted);
        if (tracked != null)
        {
            _context.Entry(tracked).CurrentValues.SetValues(excedente);
            await _context.SaveChangesAsync();
            return tracked;
        }
        return excedente;
    }

    public async Task<Excedente?> GetByIdAsync(Guid id)
    {
        return await _context.Excedentes
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
    }

    public async Task<Excedente?> GetByOrganizationAndPeriodoAsync(Guid organizationId, DateTime periodo)
    {
        return await _context.Excedentes
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.OrganizationId == organizationId && e.Periodo == periodo && !e.IsDeleted);
    }

    public async Task<List<Excedente>> GetByOrganizationAsync(Guid organizationId)
    {
        return await _context.Excedentes
            .AsNoTracking()
            .Where(e => e.OrganizationId == organizationId && !e.IsDeleted)
            .OrderByDescending(e => e.Periodo)
            .ToListAsync();
    }
}