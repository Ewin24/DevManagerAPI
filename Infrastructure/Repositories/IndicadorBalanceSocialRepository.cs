namespace Infrastructure.Repositories;

using Domain.Entities.BalanceSocial;
using Domain.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Implementación EF Core de <see cref="IIndicadorBalanceSocialRepository"/>
/// </summary>
public class IndicadorBalanceSocialRepository : IIndicadorBalanceSocialRepository
{
    private readonly DevManagerDbContext _context;

    public IndicadorBalanceSocialRepository(DevManagerDbContext context)
    {
        _context = context;
    }

    public async Task<IndicadorBalanceSocial> CreateAsync(IndicadorBalanceSocial indicador)
    {
        _context.IndicadoresBalanceSocial.Add(indicador);
        await _context.SaveChangesAsync();
        return indicador;
    }

    public async Task<IndicadorBalanceSocial> UpdateAsync(IndicadorBalanceSocial indicador)
    {
        var tracked = await _context.IndicadoresBalanceSocial
            .FirstOrDefaultAsync(i => i.Id == indicador.Id && !i.IsDeleted);
        if (tracked != null)
        {
            _context.Entry(tracked).CurrentValues.SetValues(indicador);
            await _context.SaveChangesAsync();
            return tracked;
        }
        return indicador;
    }

    public async Task<IndicadorBalanceSocial?> GetByAsociadoAndAnioAsync(Guid asociadoId, int anio)
    {
        return await _context.IndicadoresBalanceSocial
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.AsociadoId == asociadoId && i.Anio == anio && !i.IsDeleted);
    }

    public async Task<List<IndicadorBalanceSocial>> GetByOrganizationAndAnioAsync(Guid organizationId, int anio)
    {
        return await _context.IndicadoresBalanceSocial
            .AsNoTracking()
            .Where(i => i.OrganizationId == organizationId && i.Anio == anio && !i.IsDeleted)
            .ToListAsync();
    }
}