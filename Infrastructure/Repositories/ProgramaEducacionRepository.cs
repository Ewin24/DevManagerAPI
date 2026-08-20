namespace Infrastructure.Repositories;

using Domain.Entities.GestionHumana;
using Domain.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Implementación EF Core de <see cref="IProgramaEducacionRepository"/>
/// </summary>
public class ProgramaEducacionRepository : IProgramaEducacionRepository
{
    private readonly DevManagerDbContext _context;

    public ProgramaEducacionRepository(DevManagerDbContext context)
    {
        _context = context;
    }

    public async Task<ProgramaEducacion> CreateAsync(ProgramaEducacion programa)
    {
        _context.ProgramasEducacion.Add(programa);
        await _context.SaveChangesAsync();
        return programa;
    }

    public async Task<ProgramaEducacion?> GetByIdAsync(Guid id)
    {
        return await _context.ProgramasEducacion
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
    }

    public async Task<List<ProgramaEducacion>> GetByOrganizationAsync(Guid organizationId)
    {
        return await _context.ProgramasEducacion
            .AsNoTracking()
            .Where(p => p.OrganizationId == organizationId && !p.IsDeleted)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }
}